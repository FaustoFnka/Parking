// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var currentTime = document.getElementById("currtime");
function zeropadder(n) {
    return (parseInt(n, 10) < 10 ? '0' : '') + n;
}
function updateTime() {
    var timeNow = new Date(),
        hh = timeNow.getHours(),
        mm = timeNow.getMinutes(),
        ss = timeNow.getSeconds(),
        formatAMPM = (hh >= 12 ? 'PM' : 'AM');
    hh = hh % 12 || 12;
    currentTime.innerHTML = hh + "<span>:</span>" + zeropadder(mm) + "<span>:</span>" + zeropadder(ss) + " " + formatAMPM;
    setTimeout(updateTime, 1000);
}
updateTime();