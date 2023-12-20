function calculateRemainingCharacters(inputElement, maxNumber, remainingCharactersDisplayElementId) {
    var remainingCharactersDisplayElement = document.getElementById(remainingCharactersDisplayElementId);
    remainingCharactersDisplayElement.innerHTML = maxNumber - inputElement.value.length;
}

function enforceMaxLength(inputElement) {
    if(inputElement.value.length > inputElement.maxLength)
        inputElement.value = inputElement.value.substring(0, inputElement.maxLength);
}

function showHideJavascriptDependantElements(){
    var showElements = document.querySelectorAll('[js-enabled-show]');
    var hideElements = document.querySelectorAll('[js-enabled-hide]');

    showElements.forEach((obj, i)=>{
        obj.removeAttribute("hidden");
    })

    hideElements.forEach((obj, i)=>{

        obj.setAttribute("hidden",true);
    })
}

document.addEventListener("DOMContentLoaded", function(){
    showHideJavascriptDependantElements();
});

