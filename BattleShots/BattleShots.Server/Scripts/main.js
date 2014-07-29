var base_url = "http://localhost:32033/api/";
$(document).ready(function(){


    $.ajax({
        type: "POST",
        data:{
            "username" : "PiqniqJon5",
            "password" : "da39a3ee5e6b4b0d3255bfef95601890afd80709"
        },
        url: base_url + 'account/register',
        cache:false,
        success:function(result){
            console.log(result['Username']);
        //       var resultObject = JSON.parse(result);
       //     var username = resultObject['Username'];
          //  alert( username+ 'e v kru4mata');
        }
    });

});