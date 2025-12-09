<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Birthday.aspx.cs" Inherits="KShiftSmartPortalWeb.Views.Birthday" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Birthday Celebration</title>

    <!-- Google Analytics 4 -->
    <script async src="https://www.googletagmanager.com/gtag/js?id=G-4YBX6Q8Q2B"></script>
    <script>
        window.dataLayer = window.dataLayer || [];
        function gtag(){dataLayer.push(arguments);}
        gtag('js', new Date());
        gtag('config', 'G-4YBX6Q8Q2B');
    </script>

    <!-- Microsoft Clarity -->
    <script>
        (function(c,l,a,r,i,t,y){
            c[a]=c[a]||function(){(c[a].q=c[a].q||[]).push(arguments)};
            t=l.createElement(r);t.async=1;t.src="https://www.clarity.ms/tag/"+i;
            y=l.getElementsByTagName(r)[0];y.parentNode.insertBefore(t,y);
        })(window, document, "clarity", "script", "uivndhuj52");
    </script>

    <link href="https://fonts.googleapis.com/css2?family=Noto+Sans+KR:wght@400;700;900&display=swap" rel="stylesheet" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            min-height: 100vh;
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%);
            font-family: 'Noto Sans KR', sans-serif;
            overflow: hidden;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .fireworks-container {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            pointer-events: none;
            z-index: 1;
        }

        .firework {
            position: absolute;
            width: 6px;
            height: 6px;
            border-radius: 50%;
            animation: firework-explode 1.5s ease-out forwards;
        }

        @keyframes firework-explode {
            0% { transform: scale(1); opacity: 1; }
            100% { transform: scale(0); opacity: 0; }
        }

        .confetti {
            position: fixed;
            width: 12px;
            height: 12px;
            top: -20px;
            z-index: 2;
            animation: confetti-fall linear forwards;
        }

        @keyframes confetti-fall {
            0% { transform: translateY(0) rotate(0deg); opacity: 1; }
            100% { transform: translateY(100vh) rotate(720deg); opacity: 0.7; }
        }

        .main-content {
            position: relative;
            z-index: 10;
            text-align: center;
            padding: 40px;
        }

        .celebration-title {
            font-size: 1.8rem;
            color: #ffd700;
            text-shadow: 0 0 20px rgba(255, 215, 0, 0.5);
            margin-bottom: 20px;
            animation: glow 2s ease-in-out infinite alternate;
        }

        .main-message {
            font-size: 2.8rem;
            font-weight: 900;
            color: #fff;
            text-shadow:
                0 0 10px rgba(255, 255, 255, 0.8),
                0 0 20px rgba(255, 215, 0, 0.6),
                0 0 30px rgba(255, 215, 0, 0.4);
            margin-bottom: 30px;
            animation: pulse-text 2s ease-in-out infinite;
            line-height: 1.4;
        }

        .main-message .name {
            color: #ff6b6b;
            display: block;
            font-size: 3.5rem;
            animation: rainbow 3s linear infinite;
        }

        @keyframes rainbow {
            0% { color: #ff6b6b; }
            25% { color: #ffd93d; }
            50% { color: #6bcb77; }
            75% { color: #4d96ff; }
            100% { color: #ff6b6b; }
        }

        @keyframes glow {
            from { text-shadow: 0 0 20px rgba(255, 215, 0, 0.5); }
            to { text-shadow: 0 0 40px rgba(255, 215, 0, 0.9), 0 0 60px rgba(255, 215, 0, 0.5); }
        }

        @keyframes pulse-text {
            0%, 100% { transform: scale(1); }
            50% { transform: scale(1.02); }
        }

        .cake-container {
            position: relative;
            margin: 40px auto;
            animation: float 3s ease-in-out infinite;
        }

        @keyframes float {
            0%, 100% { transform: translateY(0); }
            50% { transform: translateY(-15px); }
        }

        .cake {
            font-size: 120px;
            filter: drop-shadow(0 10px 30px rgba(255, 215, 0, 0.4));
        }

        .floating-icons {
            position: fixed;
            width: 100%;
            height: 100%;
            top: 0;
            left: 0;
            pointer-events: none;
            z-index: 3;
        }

        .floating-icon {
            position: absolute;
            font-size: 40px;
            animation: float-around 6s ease-in-out infinite;
            opacity: 0.9;
        }

        @keyframes float-around {
            0%, 100% { transform: translateY(0) rotate(0deg) scale(1); }
            25% { transform: translateY(-20px) rotate(10deg) scale(1.1); }
            50% { transform: translateY(-10px) rotate(-5deg) scale(1); }
            75% { transform: translateY(-25px) rotate(5deg) scale(1.05); }
        }

        .sparkle {
            position: fixed;
            width: 10px;
            height: 10px;
            background: #fff;
            border-radius: 50%;
            animation: sparkle 1.5s ease-in-out infinite;
            z-index: 4;
        }

        @keyframes sparkle {
            0%, 100% { transform: scale(0); opacity: 0; }
            50% { transform: scale(1); opacity: 1; box-shadow: 0 0 20px #fff, 0 0 40px #ffd700; }
        }

        .heart {
            position: fixed;
            font-size: 24px;
            animation: heart-float 4s ease-in-out forwards;
            z-index: 5;
            opacity: 0;
        }

        @keyframes heart-float {
            0% { transform: translateY(0) scale(0); opacity: 0; }
            20% { opacity: 1; transform: translateY(-50px) scale(1); }
            100% { transform: translateY(-300px) scale(0.5); opacity: 0; }
        }

        .banner {
            display: flex;
            justify-content: center;
            gap: 10px;
            margin-top: 30px;
            flex-wrap: wrap;
        }

        .banner-letter {
            display: inline-block;
            font-size: 2rem;
            font-weight: bold;
            padding: 10px 15px;
            background: linear-gradient(135deg, #ff6b6b, #ffd93d);
            color: #fff;
            border-radius: 10px;
            animation: bounce 0.6s ease-in-out infinite;
            box-shadow: 0 5px 20px rgba(255, 107, 107, 0.4);
        }

        @keyframes bounce {
            0%, 100% { transform: translateY(0); }
            50% { transform: translateY(-10px); }
        }

        .banner-letter:nth-child(1) { animation-delay: 0s; }
        .banner-letter:nth-child(2) { animation-delay: 0.1s; }
        .banner-letter:nth-child(3) { animation-delay: 0.2s; }
        .banner-letter:nth-child(4) { animation-delay: 0.3s; }
        .banner-letter:nth-child(5) { animation-delay: 0.4s; }
        .banner-letter:nth-child(6) { animation-delay: 0.5s; }

        .ribbon-left, .ribbon-right {
            position: fixed;
            top: 0;
            width: 150px;
            height: 100%;
            background: linear-gradient(180deg, transparent 0%, rgba(255, 107, 107, 0.1) 50%, transparent 100%);
            z-index: 0;
        }

        .ribbon-left { left: 0; }
        .ribbon-right { right: 0; }

        .star {
            position: fixed;
            color: #ffd700;
            font-size: 20px;
            animation: twinkle 2s ease-in-out infinite;
            z-index: 2;
        }

        @keyframes twinkle {
            0%, 100% { opacity: 0.3; transform: scale(0.8); }
            50% { opacity: 1; transform: scale(1.2); }
        }

        @media (max-width: 768px) {
            .main-message { font-size: 1.8rem; }
            .main-message .name { font-size: 2.2rem; }
            .cake { font-size: 80px; }
            .celebration-title { font-size: 1.3rem; }
            .banner-letter { font-size: 1.2rem; padding: 8px 12px; }
            .floating-icon { font-size: 28px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="ribbon-left"></div>
        <div class="ribbon-right"></div>
        <div class="fireworks-container" id="fireworksContainer"></div>
        <div class="floating-icons" id="floatingIcons"></div>

        <div class="main-content">
            <div class="celebration-title" id="titleText"></div>
            <div class="main-message">
                <span class="name" id="nameText"></span>
                <span id="msgText"></span>
            </div>
            <div class="cake-container">
                <div class="cake" id="cakeIcon"></div>
            </div>
            <div class="banner" id="bannerContainer"></div>
        </div>

        <div id="stars"></div>
    </form>

    <script>
        // Unicode safe text insertion
        document.getElementById('titleText').innerHTML = '\uD83C\uDF8A HAPPY BIRTHDAY \uD83C\uDF8A';
        document.getElementById('nameText').textContent = '\uBC15\uC911\uC6A9 \uBD80\uC7A5\uB2D8\uC758';
        document.getElementById('msgText').textContent = '\uD0C4\uC0DD\uC77C\uC744 \uCD95\uD558\uB4DC\uB9BD\uB2C8\uB2E4!';
        document.getElementById('cakeIcon').textContent = '\uD83C\uDF82';

        // Banner letters
        var bannerLetters = ['\uCD95', '\uD558', '\uD569', '\uB2C8', '\uB2E4', '!'];
        var bannerContainer = document.getElementById('bannerContainer');
        bannerLetters.forEach(function(letter) {
            var span = document.createElement('span');
            span.className = 'banner-letter';
            span.textContent = letter;
            bannerContainer.appendChild(span);
        });

        // Floating icons
        var icons = ['\uD83C\uDF88', '\uD83C\uDF81', '\uD83C\uDF80', '\uD83C\uDF8A', '\uD83C\uDF89', '\u2B50', '\u2728', '\uD83D\uDC9D', '\uD83C\uDF1F', '\uD83C\uDFB5', '\uD83C\uDFB6', '\uD83D\uDCAB', '\uD83E\uDD73', '\uD83C\uDF70'];
        var floatingContainer = document.getElementById('floatingIcons');

        for (var i = 0; i < 20; i++) {
            var icon = document.createElement('div');
            icon.className = 'floating-icon';
            icon.textContent = icons[Math.floor(Math.random() * icons.length)];
            icon.style.left = Math.random() * 100 + '%';
            icon.style.top = Math.random() * 100 + '%';
            icon.style.animationDelay = Math.random() * 5 + 's';
            icon.style.animationDuration = (4 + Math.random() * 4) + 's';
            floatingContainer.appendChild(icon);
        }

        // Stars
        var starsContainer = document.getElementById('stars');
        for (var i = 0; i < 30; i++) {
            var star = document.createElement('div');
            star.className = 'star';
            star.textContent = '\u2B50';
            star.style.left = Math.random() * 100 + '%';
            star.style.top = Math.random() * 100 + '%';
            star.style.animationDelay = Math.random() * 3 + 's';
            star.style.fontSize = (10 + Math.random() * 20) + 'px';
            starsContainer.appendChild(star);
        }

        // Sparkles
        function createSparkle() {
            var sparkle = document.createElement('div');
            sparkle.className = 'sparkle';
            sparkle.style.left = Math.random() * 100 + '%';
            sparkle.style.top = Math.random() * 100 + '%';
            sparkle.style.animationDelay = Math.random() * 0.5 + 's';
            document.body.appendChild(sparkle);
            setTimeout(function() { sparkle.remove(); }, 1500);
        }
        setInterval(createSparkle, 200);

        // Confetti
        var confettiColors = ['#ff6b6b', '#ffd93d', '#6bcb77', '#4d96ff', '#ff9ff3', '#ffeaa7', '#dfe6e9', '#fd79a8'];
        function createConfetti() {
            var confetti = document.createElement('div');
            confetti.className = 'confetti';
            confetti.style.left = Math.random() * 100 + '%';
            confetti.style.backgroundColor = confettiColors[Math.floor(Math.random() * confettiColors.length)];
            confetti.style.animationDuration = (2 + Math.random() * 3) + 's';
            var shapes = ['50%', '0%'];
            confetti.style.borderRadius = shapes[Math.floor(Math.random() * shapes.length)];
            if (Math.random() > 0.5) {
                confetti.style.width = '8px';
                confetti.style.height = '16px';
            }
            document.body.appendChild(confetti);
            setTimeout(function() { confetti.remove(); }, 5000);
        }
        setInterval(createConfetti, 100);

        // Fireworks
        function createFirework(x, y) {
            var colors = ['#ff6b6b', '#ffd93d', '#6bcb77', '#4d96ff', '#ff9ff3', '#fff'];
            var particles = 30;
            for (var i = 0; i < particles; i++) {
                var particle = document.createElement('div');
                particle.className = 'firework';
                particle.style.left = x + 'px';
                particle.style.top = y + 'px';
                particle.style.backgroundColor = colors[Math.floor(Math.random() * colors.length)];
                var angle = (360 / particles) * i;
                var velocity = 50 + Math.random() * 100;
                var vx = Math.cos(angle * Math.PI / 180) * velocity;
                var vy = Math.sin(angle * Math.PI / 180) * velocity;
                particle.style.animation = 'none';
                particle.style.transition = 'all 1.5s ease-out';
                document.getElementById('fireworksContainer').appendChild(particle);
                (function(p, vx, vy) {
                    requestAnimationFrame(function() {
                        p.style.transform = 'translate(' + vx + 'px, ' + vy + 'px)';
                        p.style.opacity = '0';
                    });
                })(particle, vx, vy);
                setTimeout((function(p) { return function() { p.remove(); }; })(particle), 1500);
            }
        }

        function autoFirework() {
            var x = Math.random() * window.innerWidth;
            var y = Math.random() * (window.innerHeight * 0.6);
            createFirework(x, y);
        }
        setInterval(autoFirework, 1500);
        setTimeout(autoFirework, 500);
        setTimeout(autoFirework, 800);
        setTimeout(autoFirework, 1100);

        // Hearts
        var hearts = ['\u2764\uFE0F', '\uD83D\uDC95', '\uD83D\uDC96', '\uD83D\uDC97', '\uD83D\uDC9D'];
        function createHeart() {
            var heart = document.createElement('div');
            heart.className = 'heart';
            heart.textContent = hearts[Math.floor(Math.random() * hearts.length)];
            heart.style.left = Math.random() * 100 + '%';
            heart.style.bottom = '0';
            document.body.appendChild(heart);
            setTimeout(function() { heart.remove(); }, 4000);
        }
        setInterval(createHeart, 800);

        // Click fireworks
        document.addEventListener('click', function(e) {
            createFirework(e.clientX, e.clientY);
        });
    </script>
</body>
</html>
