$(document).ready(function () {
    $('#submitWeather').click(function () {

        var city = $("#city").val();

        if (city != '') {

            $.ajax({

                url: "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&units=metric" + "&APPID=c10bb3bd22f90d636baa008b1529ee25",
                type: "GET",
                dataType: "jsonp",
                success: function (data) {
                    console.log(data);
                }


            });

        } else {
            $("#error").html('Field cannot be empty');
        }


    });
});