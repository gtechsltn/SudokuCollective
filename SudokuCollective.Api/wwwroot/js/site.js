// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    var date = new Date();
    $("#year").text(date.getFullYear());

    checkAPI();

    setInterval(function () {

        checkAPI();

    }, 10000);
});

function checkAPI() {

    axios.get("api/helloworld")
        .then((response) => {
            console.log("hello world response: ", response);

            var apiUp;

            if (response.data.isSuccess) {

                apiUp = true;

            }

            if (apiUp) {

                $("#apiMessage")
                    .text("The Sudoku Collective API is up and running!");

                if ($("#apiMessage").hasClass("red")) {

                    $("#apiMessage").removeClass("red");
                }

                $("#apiMessage").show();

            }
        }, (error) => {
            let e = JSON.parse(JSON.stringify(error));
            console.log("hello world error: ", e);

            $("#apiMessage")
                .text("There is an issue with the Sudoku Collective API: "
                    + e.message);

            if (!$("#apiMessage").hasClass("red")) {

                $("#apiMessage").addClass("red");
            }

            $("#apiMessage").show();
        });
}