// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", () => {

    const btnSwap = document.getElementById("btn-swap-languages");
    if (!btnSwap) return;

    btnSwap.addEventListener("click", () => {

        const fromSelect = document.getElementById('lang-from');
        const toSelect = document.getElementById('lang-to');
        const source = document.getElementById('source-textarea');
        const translation = document.getElementById('translation-textarea');
        const form = btnSwap.closest('form');
        const submitBtn = form.querySelector('button[name="action-button"]');     

        if (!fromSelect || !toSelect || !source || !translation || !form) return;

        const fromLang = fromSelect.value;
        const toLang = toSelect.value;
        const translatedText = translation.value.trim();

        if (!translatedText) {
            fromSelect.value = toLang;
            toSelect.value = fromLang;
            return;
        }

        fromSelect.value = toLang;
        toSelect.value = fromLang;

        source.value = translatedText;

        translation.value = "";

        form.requestSubmit(submitBtn);
    });
});

