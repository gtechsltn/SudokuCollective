// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.addEventListener('load', () => {

    const date = new Date();
    
    document.getElementById('year').innerHTML = date.getFullYear();

    const htmlElement = document.getElementById('apiMessage');

    checkAPI(htmlElement);

    setInterval(async () => {

        await checkAPI(htmlElement);

    }, 10000, htmlElement);
}, false);

async function checkAPI(htmlElement) {

    try {
        const response = await fetch("api/helloworld");

        const data = await response.json();

        if (data.isSuccess) {

            htmlElement.innerHTML = 'The Sudoku Collective API is up and running!';

            if (htmlElement.classList.contains('red')) {

                htmlElement.classList.remove("red");
            }

            htmlElement.style.display = 'block';
        }

    } catch (error) {
        console.log("hello world error: ", error);
        
        htmlElement.innerHTML = 'The Sudoku Collective API is down.';

        if (!htmlElement.classList.contains('red')) {

            htmlElement.classList.add("red");
        }

        htmlElement.style.display = 'block';
    }
}
