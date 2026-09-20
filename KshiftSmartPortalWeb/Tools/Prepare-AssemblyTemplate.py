"""조립 템플릿 배포 사본에서 외부 수식 및 사용하지 않는 문자열 캐시 제거."""
import argparse
import re
import xml.etree.ElementTree as ET
from pathlib import Path
from zipfile import ZipFile, ZIP_DEFLATED

NS = {"m": "http://schemas.openxmlformats.org/spreadsheetml/2006/main"}


def prepare(source, destination):
    if source.resolve() == destination.resolve():
        raise ValueError("ALIS 원본과 배포 사본 경로가 같을 수 없습니다.")
    with ZipFile(source) as archive:
        parts = {name: archive.read(name) for name in archive.namelist()}
    removed = 0
    cell_pattern = r"<c\b[^>]*?(?:/>|>.*?</c>)"
    used_strings = set()
    sheets = [name for name in parts if re.fullmatch(r"xl/worksheets/sheet\d+\.xml", name)]
    for name in sheets:
        xml = parts[name].decode("utf-8")
        root = ET.fromstring(xml)
        external_shared = {
            f.get("si") for f in root.findall(".//m:f", NS)
            if "[" in (f.text or "") and f.get("t") == "shared"
        }
        def clean_cell(match):
            nonlocal removed
            cell = match.group()
            element = ET.fromstring(cell.replace("<c ", '<c xmlns="' + NS["m"] + '" ', 1))
            formula = element.find("m:f", NS)
            if formula is not None:
                external = "[" in (formula.text or "") or (
                    formula.get("t") == "shared" and formula.get("si") in external_shared
                )
                # 외부 수식은 캐시값으로 고정하지 않고 빈 셀로 남긴다.
                if external:
                    cell = re.sub(r"<f\b[^>]*?(?:/>|>.*?</f>)", "", cell, flags=re.S)
                    cell = re.sub(r'\s+t="[^"]*"', "", cell, count=1)
                    removed += 1
                # 내부 수식도 오래된 계산값을 버리고 DevExpress에서 다시 계산한다.
                cell = re.sub(r"<v\b[^>]*?(?:/>|>.*?</v>)", "", cell, flags=re.S)
            elif element.get("t") == "s":
                used_strings.add(int(element.find("m:v", NS).text))
            return cell

        parts[name] = re.sub(cell_pattern, clean_cell, xml, flags=re.S).encode("utf-8")

    # 셀에서 사용하지 않는 문자열이 남아 있다면 배포 사본에서 제외한다.
    strings = parts["xl/sharedStrings.xml"].decode("utf-8")
    entries = re.findall(r"<si\b[^>]*>.*?</si>", strings, flags=re.S)
    mapping = {old: new for new, old in enumerate(sorted(used_strings))}
    count = 0
    for name in sheets:
        def remap(match):
            nonlocal count
            cell = match.group()
            if re.search(r'\bt="s"', cell):
                count += 1
                cell = re.sub(r"<v>(\d+)</v>", lambda value: '<v>' + str(mapping[int(value[1])]) + '</v>', cell)
            return cell
        parts[name] = re.sub(cell_pattern, remap, parts[name].decode("utf-8"), flags=re.S).encode("utf-8")
    header = strings[:strings.index(">", strings.index("<sst")) + 1]
    header = re.sub(r'\bcount="\d+"', 'count="' + str(count) + '"', header)
    header = re.sub(r'\buniqueCount="\d+"', 'uniqueCount="' + str(len(mapping)) + '"', header)
    parts["xl/sharedStrings.xml"] = (header + ''.join(entries[i] for i in sorted(used_strings)) + '</sst>').encode("utf-8")
    # 삭제된 수식의 계산 순서 참조를 제거한다. Excel/DevExpress가 다음 계산 때 재생성한다.
    parts.pop('xl/calcChain.xml', None)
    for name, pattern in (
        ('xl/_rels/workbook.xml.rels', r'<Relationship\b[^>]*Type="[^"]*/calcChain"[^>]*/>'),
        ('[Content_Types].xml', r'<Override\b[^>]*PartName="/xl/calcChain.xml"[^>]*/>'),
    ):
        parts[name] = re.sub(pattern, '', parts[name].decode('utf-8')).encode('utf-8')
    destination.parent.mkdir(parents=True, exist_ok=True)
    with ZipFile(destination, "w", ZIP_DEFLATED) as archive:
        for name, data in parts.items():
            archive.writestr(name, data)
    print(f"External formula cells cleared: {removed}; shared strings retained: {len(mapping)}/{len(entries)}")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("source", type=Path)
    parser.add_argument("destination", type=Path)
    args = parser.parse_args()
    prepare(args.source, args.destination)
