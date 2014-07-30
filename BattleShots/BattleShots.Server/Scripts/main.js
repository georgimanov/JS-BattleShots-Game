var baseUrl = "http://localhost:32033/api/";
//var baseUrl = "http://battleshots-1.apphb.com/api/";
var gameAcces = false;
var selectBar = false;
var userName = localStorage.getItem('userName');
if(userName == null){
    userName = "Example User";
}
$(document).ready(function(){
    checkGameAcces();
});

function checkGameAcces(){
    if(localStorage.getItem("sessionKey") == null){
        showLogin();
    }else{
       if(selectBar == true){
           showArena();
       }else{
           showBars();
       }
    }
}

function showLogin(){
    $('.basic-element').hide();
    $('#login-box').fadeIn(1200);
}
function showJoin(){
    $('.basic-element').hide();
    $('#join-box').fadeIn(1200);
}
function showBarBox(){
    $('.basic-element').hide();
    $('#bars-box').fadeIn(1200);
}
function showArena(){
    $('.basic-element').hide();
    $('#arena').fadeIn(1200);
}
function showBars(){
    $('.basic-element').hide();
    $('#bars-box').fadeIn(1200);
}
function joinTable(){

}
function registerUser(){
    var loginName = $('#username-join').val();
    var userPass = $('#password-join').val();
    var userPass2 = $('#password-2-join').val();
    if(userPass === userPass2){
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
    }else{
        showError('Incorrect password!');
    }

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
            userName = localStorage.getItem('userName');
            checkGameAcces();

        },function(error){
            console.log(error);
            showError(error);
        });
}


function showError(error){
    var text = '';
    if(typeof(error) == "object"){
            var err  = JSON.parse(error.responseText);
        text = err['Message'];
    }else{
        text = error;
    }
    $('#error-dialog').html(text).dialog({modal:true});
}

function logOut(){
    var sk =  localStorage.getItem('sessionKey');
    var headers = {
        'X-SessionKey' :sk
    }
    httpRequester.putJson(baseUrl+'account/logout',{},headers)
        .then(function(data){
            localStorage.removeItem('userName');
            localStorage.removeItem('sessionKey');
            location.reload();
            checkGameAcces();
        },function(error){
            showError(error);
        });
}