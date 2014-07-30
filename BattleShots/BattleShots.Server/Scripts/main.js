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
    $('#rooms-lobby').fadeIn(1200);
}
function showBars(){
     userName = localStorage.getItem('userName');
    $('#login-box').addClass('hidden');
    $('#bars-box').removeClass('hidden');
    $('#username').html(userName);
    $('#error-dialog').dialog('close');
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
        alert('ne6to');
        showError({"responseText" : {"Message":"Invalid password."}});
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

            checkGameAcces();
        },function(error){
            showError(error);
        });
}


function showError(error){
    console.lo9
    var err = JSON.parse(error['responseText']['Message']);
   console.log(err);
    $('#error-dialog').html(err).dialog({modal:true});
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