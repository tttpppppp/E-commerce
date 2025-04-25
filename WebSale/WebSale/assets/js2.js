function countdownRedirect() {
    var seconds = 10;
    var countdownElement = document.getElementById('countdown-timer');
    function updateCountdown() {
        seconds--;
        countdownElement.textContent = seconds;
        if (seconds <= 0) {
            clearInterval(timer);
            window.location.href = '/';
        }
    }
    var timer = setInterval(updateCountdown, 1000);
}

window.onload = function () {
    countdownRedirect();
};
countdownRedirect();