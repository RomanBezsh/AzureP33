// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const btnSwapLanguages = document.getElementById("btn-swap-languages");

btnSwapLanguages.addEventListener("click", () => swapLanguages());

const swapLanguages = () => {
    const langFrom = document.getElementsByName("lang-from")[0];
    const langTo = document.getElementsByName("lang-to")[0];

    const temp = langFrom.value;
    langFrom.value = langTo.value;
    langTo.value = temp;
};
