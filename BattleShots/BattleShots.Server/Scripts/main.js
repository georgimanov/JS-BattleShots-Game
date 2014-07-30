var baseUrl = "http://localhost:32033/api/";
//var baseUrl = "http://battleshots-1.apphb.com/api/";
var gameAcces = false;
var userName = localStorage.getItem('userName');
if(userName == null){
    userName = "Example User";
}
$(document).ready(function(){
    checkGameAcces();
});

function checkGameAcces(){
    if(localStorage.getItem("sessionKey") == null){
        $('#dialog').dialog({modal:true});
    }else{
        RunGame();
    }
}

function RunGame(){
     userName = localStorage.getItem('userName');
    $('#username').html(userName);
    $('#dialog').dialog('close');
    $('#error-dialog').dialog('close');

}
function registerUser(){
    var loginName = $('#username-join').val();
    var userPass = $('#password-join').val();
    var pass = SHA1(userPass);

    var data = {
        "username" : loginName,
        "password" : pass
    };

    httpRequester.postJson(baseUrl+'account/register',data,{})
        .then(function(data){
         var DataUserName = data['Username'];
         var dataSessionKey = data['SessionKey'];
         localStorage.setItem('userName',DataUserName);
         localStorage.setItem('sessionKey',dataSessionKey);

         checkGameAcces();
            },function(error){
        showError(error);
        });


}

function checkLogin(){

    var loginName = $('#username-login').val();
    var userPass = $('#password-login').val();
    var passSha1 = SHA1(userPass);
    var data = {
        'username': loginName,
        'password':passSha1
    };
    httpRequester.postJson(baseUrl+'account/login',data,{})
        .then(function(data){

            var DataUserName = data['Username'];
            var dataSessionKey = data['SessionKey'];
            localStorage.setItem('userName',DataUserName);
            localStorage.setItem('sessionKey',dataSessionKey);

            checkGameAcces();
        },function(error){
            showError(error);
        });
}


function showError(error){
    var responseText = JSON.parse(error['responseText']);
    var text = responseText.Message;
    $('#error-dialog').html(text).dialog({modal:true});
}

function logOut(){
    var sk =  localStorage.getItem('sessionKey');
    var headers = {
        'X-SessionKey' :sk
    }
    httpRequester.putJson(baseUrl+'account/logout',{},headers)
        .then(function(data){
            alert('data: ' + data);
            localStorage.removeItem('userName');
            localStorage.removeItem('sessionKey');
            location.reload();
            checkGameAcces();
        },function(error){
            showError(error);
        });
}