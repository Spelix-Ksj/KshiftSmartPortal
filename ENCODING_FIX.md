# 🔧 인코딩 문제 해결 가이드

## 문제 증상

```
파서 오류 메시지: 'DevExpress.Web.ListEditItemCollection' 안에서는 
리터럴 내용('<dx:ListEditItem Text="吏?? Value="B" />')을 사용할 수 없습니다.
```

한글이 깨져서 표시되는 문제입니다.

---

## ✅ 해결 방법 (이미 수정됨)

**현재 제공되는 ZIP 파일은 이미 UTF-8 BOM으로 저장되어 있어 별도 작업이 필요 없습니다.**

하지만 파일을 수정하거나 새로 만들 때는 아래 방법을 따라주세요.

---

## 📝 Visual Studio에서 인코딩 확인/변경

### 방법 1: Visual Studio 기본 설정 (권장)

1. **파일 열기** (Default.aspx)
2. **파일 > 다른 이름으로 저장** 클릭
3. **저장 버튼 옆 화살표** 클릭
4. **인코딩하여 저장** 선택
5. **유니코드(UTF-8 서명 포함) - 코드 페이지 65001** 선택
6. **확인** 클릭

### 방법 2: 고급 저장 옵션

1. 파일 열기 (Default.aspx)
2. **파일 > 고급 저장 옵션**
   - 메뉴가 안 보이면: 도구 > 사용자 지정 > 명령 > 파일 > 고급 저장 옵션 추가
3. **유니코드(UTF-8 서명 포함) - 코드 페이지 65001** 선택
4. **확인** 클릭

---

## 🔍 인코딩 확인 방법

### Visual Studio에서 확인
1. 파일 열기
2. 상태 표시줄 오른쪽 하단 확인
3. **UTF-8 BOM** 또는 **유니코드(UTF-8 서명 포함)** 표시 확인

### Notepad++에서 확인
1. 파일 열기
2. 인코딩 메뉴 확인
3. **UTF-8-BOM**이 체크되어 있어야 함

---

## 🚨 주의사항

### ASP.NET Web Forms에서 필요한 인코딩

| 파일 유형 | 필수 인코딩 |
|----------|-----------|
| .aspx | UTF-8 BOM |
| .ascx | UTF-8 BOM |
| .master | UTF-8 BOM |
| .cs | UTF-8 BOM (권장) |
| .config | UTF-8 BOM (권장) |

### ❌ 잘못된 인코딩
- UTF-8 (BOM 없음) ← 한글 깨짐
- ANSI / Windows-1252
- EUC-KR

### ✅ 올바른 인코딩
- **UTF-8 BOM** (UTF-8 with BOM)
- 유니코드 (UTF-8 서명 포함)

---

## 🛠️ PowerShell로 일괄 변환

모든 파일을 한 번에 UTF-8 BOM으로 변환:

```powershell
# PowerShell 스크립트
Get-ChildItem -Path . -Include *.aspx,*.cs,*.config -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $utf8BOM = New-Object System.Text.UTF8Encoding $true
    [System.IO.File]::WriteAllText($_.FullName, $content, $utf8BOM)
    Write-Host "Converted: $($_.Name)"
}
```

---

## 🐍 Python으로 일괄 변환

```python
import os
import codecs

def convert_to_utf8_bom(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    with open(file_path, 'w', encoding='utf-8-sig') as f:
        f.write(content)

# 모든 .aspx, .cs, .config 파일 변환
for root, dirs, files in os.walk('.'):
    for file in files:
        if file.endswith(('.aspx', '.cs', '.config')):
            file_path = os.path.join(root, file)
            convert_to_utf8_bom(file_path)
            print(f"Converted: {file_path}")
```

---

## 🔄 이미 깨진 파일 복구

한글이 이미 깨진 경우:

### 1단계: 원본 내용 확인
깨지기 전 내용을 알고 있다면 직접 수정

### 2단계: 파일 다시 다운로드
제공된 ZIP 파일에서 다시 추출

### 3단계: 인코딩 올바르게 저장
UTF-8 BOM으로 저장

---

## 📋 체크리스트

새 파일을 만들거나 수정할 때:

- [ ] Default.aspx → UTF-8 BOM
- [ ] Default.aspx.cs → UTF-8 BOM
- [ ] Default.aspx.designer.cs → UTF-8 BOM
- [ ] Web.config → UTF-8 BOM
- [ ] 모든 .cs 파일 → UTF-8 BOM
- [ ] 한글이 제대로 표시되는지 확인

---

## 💡 팁

### Visual Studio 기본 인코딩 설정

1. **도구 > 옵션**
2. **환경 > 문서**
3. **"서명 없이 유니코드 데이터를 저장할 수 없는 경우 UTF-8 인코딩으로 저장"** 체크
4. **확인**

### Git 설정 (.gitattributes)

프로젝트 루트에 `.gitattributes` 파일 생성:

```
*.aspx text eol=crlf encoding=utf-8-bom
*.ascx text eol=crlf encoding=utf-8-bom
*.master text eol=crlf encoding=utf-8-bom
*.cs text eol=crlf encoding=utf-8-bom
*.config text eol=crlf encoding=utf-8-bom
```

---

## ❓ FAQ

### Q: UTF-8과 UTF-8 BOM의 차이는?
**A:** 
- **UTF-8**: BOM(Byte Order Mark) 없음
- **UTF-8 BOM**: 파일 시작 부분에 EF BB BF 바이트 추가
- ASP.NET은 BOM이 있어야 한글을 올바르게 인식

### Q: 왜 Visual Studio는 기본적으로 UTF-8 BOM을 사용하지 않나요?
**A:** 
- Visual Studio는 파일을 열 때 자동으로 인코딩 감지
- 하지만 한글이 포함된 ASP.NET 파일은 명시적으로 UTF-8 BOM 필요

### Q: 모든 파일을 UTF-8 BOM으로 해야 하나요?
**A:**
- **.aspx, .ascx, .master**: 필수
- **.cs, .config**: 권장 (한글 주석이 있는 경우)
- **.js, .css**: UTF-8 (BOM 없음) 권장

---

## 🎯 요약

1. **현재 ZIP 파일**: 이미 UTF-8 BOM으로 저장됨 ✅
2. **새 파일 만들 때**: Visual Studio에서 UTF-8 BOM으로 저장
3. **수정할 때**: 저장 시 인코딩 확인
4. **문제 발생 시**: 이 가이드 참조

---

**문제가 해결되지 않으면 ZIP 파일을 다시 다운로드하세요!**
