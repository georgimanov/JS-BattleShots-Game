var baseUrl = "http://localhost:32033/api/";
//var baseUrl = "http://battleshots-1.apphb.com/api/";
var gameAcces = false;
var selectBar = false;
var userName = localStorage.getItem('userName');
if(userName == null){
var animationSpeed = 1200;
if (userName == null) {
    userName = "Example User";
}
$(document).ready(function(){
$(document).ready(function () {

    checkGameAcces();
});

function checkGameAcces(){
    if(localStorage.getItem("sessionKey") == null){
        $('#dialog').dialog({modal:true});
    }else{
        RunGame();
function checkGameAcces() {
    if (localStorage.getItem("sessionKey") == null) {
        showLogin();
    } else {
        userName = localStorage.getItem('userName');
        $('#username').html(userName);
        $('#user-box').fadeIn(animationSpeed);
        if (selectBar == true) {
            showArena();
        } else {
            showBars();
        }
    }
}

function RunGame(){
     userName = localStorage.getItem('userName');
    $('#username').html(userName);
    $('#dialog').dialog('close');
    $('#error-dialog').dialog('close');
function showLogin() {
    $('.basic-element').hide();
    $('#login-box').fadeIn(animationSpeed);
}
function showJoin() {
    $('.basic-element').hide();
    $('#join-box').fadeIn(animationSpeed);
}
function showBarBox() {
    $('.basic-element').hide();
    $('#bars-box').fadeIn(animationSpeed);
}
function showArena() {
    $('.basic-element').hide();
    $('#arena').fadeIn(animationSpeed);
}
function showBars() {
    $('.basic-element').hide();
    $('#bars-box').fadeIn(animationSpeed);
}
function joinTable() {

}
function registerUser(){
function registerUser() {
    var loginName = $('#username-join').val();
    var userPass = $('#password-join').val();
    var pass = SHA1(userPass);
    var userPass2 = $('#password-2-join').val();
    if (userPass === userPass2) {
        var pass = SHA1(userPass);

    var data = {
        "username" : loginName,
        "password" : pass
    };
        var data = {
            "username": loginName,
            "password": pass
        };
        httpRequester.postJson(baseUrl + 'account/register', data, {})
            .then(function (data) {
                var DataUserName = data['Username'];
                var dataSessionKey = data['SessionKey'];
                localStorage.setItem('userName', DataUserName);
                localStorage.setItem('sessionKey', dataSessionKey);

    httpRequester.postJson(baseUrl+'account/register',data,{})
        .then(function(data){
         var DataUserName = data['Username'];
         var dataSessionKey = data['SessionKey'];
         localStorage.setItem('userName',DataUserName);
         localStorage.setItem('sessionKey',dataSessionKey);
                checkGameAcces();
            }, function (error) {
                showError(error);
            });
    } else {
        showError('Incorrect password!');
    }

         checkGameAcces();
            },function(error){
        showError(error);
        });








}

function checkLogin(){

function checkLogin() {

    var loginName = $('#username-login').val();
    var userPass = $('#password-login').val();
    var passSha1 = SHA1(userPass);
    var data = {
        'username': loginName,
        'password':passSha1
        'password': passSha1
    };
    httpRequester.postJson(baseUrl+'account/login',data,{})
        .then(function(data){
    httpRequester.postJson(baseUrl + 'account/login', data, {})
        .then(function (data) {

            var DataUserName = data['Username'];
            var dataSessionKey = data['SessionKey'];
            localStorage.setItem('userName',DataUserName);
            localStorage.setItem('sessionKey',dataSessionKey);
            localStorage.setItem('userName', DataUserName);
            localStorage.setItem('sessionKey', dataSessionKey);

            checkGameAcces();
        },function(error){

        }, function (error) {
            console.log(error);
            showError(error);
        });
}


function showError(error){
    var responseText = JSON.parse(error['responseText']);
    var text = responseText.Message;
    $('#error-dialog').html(text).dialog({modal:true});

function showError(error) {
    var text = '';
    if (typeof(error) == "object") {
        var err = JSON.parse(error.responseText);
        text = err['Message'];
    } else {
        text = error;
    }
    $('#error-dialog').html(text).dialog({modal: true});
}

function logOut(){
    var sk =  localStorage.getItem('sessionKey');
function logOut() {
    var sk = localStorage.getItem('sessionKey');
    var headers = {
        'X-SessionKey' :sk
        'X-SessionKey': sk
    }
    httpRequester.putJson(baseUrl+'account/logout',{},headers)
        .then(function(data){
            alert('data: ' + data);
    httpRequester.putJson(baseUrl + 'account/logout', {}, headers)
        .then(function (data) {
            $('#user-box').fadeOut(animationSpeed);
            localStorage.removeItem('userName');
            localStorage.removeItem('sessionKey');
            location.reload();
            checkGameAcces();
        },function(error){
        }, function (error) {
            showError(error);
        });
}