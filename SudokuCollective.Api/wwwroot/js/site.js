// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.addEventListener('load', async () => {

    const date = new Date();
    
    document.getElementById('year').innerHTML = date.getFullYear();

    const htmlElement = document.getElementById('apiMessage');

    await checkAPI(htmlElement);

    setInterval(async () => {

        await checkAPI(htmlElement);

    }, 10000, htmlElement);

}, false);

async function checkAPI(htmlElement) {

    try {
        
        const response = await fetch("api/helloworld");

        const data = await response.json();

        let message;

        if (data.isSuccess) {

            message = 'The Sudoku Collective API is up and running!';

        } else {

            message = data.message;
        }

        updateIndex(htmlElement, message, data.isSuccess);

    } catch (error) {
        
        updateIndex(
            htmlElement, 
            'Error connecting to Sudoku Collective API: <br/>' + error, 
            false);
    }
}

function updateIndex(htmlElement, message, isSuccess) {

    if (htmlElement.innerHTML !== undefined 
        && htmlElement.classList !== undefined 
        && typeof(message) === 'string' 
        && typeof(isSuccess) === 'boolean') {

        htmlElement.innerHTML = message;

        if (isSuccess) {
    
            if (htmlElement.classList.contains('red')) {
        
                htmlElement.classList.remove("red");
            }
    
        } else {
    
            if (!htmlElement.classList.contains('red')) {
    
                htmlElement.classList.add("red");
            }
        }

    } else {

        if (typeof(message) !== 'string') {
            message = 'message invalid';
        }

        console.log("Invalid HTMLElement for message: " + message);
    }
}
