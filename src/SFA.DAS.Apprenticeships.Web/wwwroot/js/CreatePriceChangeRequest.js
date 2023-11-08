function calculateTotal() {
    var trainingPrice = document.getElementById("ApprenticeshipTrainingPrice").value;
    var epaPrice = document.getElementById("ApprenticeshipEndPointAssessmentPrice").value;
    var total = 0;
    if (trainingPrice != null && trainingPrice != "") {
        total += parseInt(trainingPrice);
    }
    if (epaPrice != null && epaPrice != "") {
        total += parseInt(epaPrice);
    }
    document.getElementById("ApprenticeshipTotalPrice").innerHTML = '\u00A3' + total;
}