
var result2 = {
    MyBoard: "00bBBB00000000000000000P000000000P00000000000000000000AAAAA000000000000000000DDD0000000000000000SSS0",
    OpponentBoardBoard: "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
};

function DrowArena(idPattern,obj,param ){

//var myBoardArr = [];
    for (var i = 0 ;i < 10; i++) {
//    myBoardArr[i] = [];
        for (var j = 0; j < 10; j++) {
//         myBoardArr[i][j] = result2.MyBoard[i * 10 + j];
            var MyBoardChar = (obj[param][i * 10 + j]).toString();
            var myBoardId = idPattern+"-"+(i+1)+"-"+ (j+1);
            switch(MyBoardChar){
                case "0" : break;
                case "~" : $('#'+myBoardId).addClass('hit-water');
                    break;
                case "*" : $('#'+myBoardId).addClass('hit-ship');
                    break;
                case "A" : $('#'+myBoardId).addClass('aircraft-carrier');
                    break;
                case "B" : $('#'+myBoardId).addClass('battleship');
                    break;
                case "S" : $('#'+myBoardId).addClass('submarine');
                    break;
                case "D" : $('#'+myBoardId).addClass('destroyer');
                    break;
                case "P" : $('#'+myBoardId).addClass('patrol-boat');
                    break;
                case "a" : $('#'+myBoardId).addClass('aircraft-carrier-demage');
                    break;
                case "b" : $('#'+myBoardId).addClass('battleship-demage');
                    break;
                case "c" : $('#'+myBoardId).addClass('submarine-demage');
                    break;
                case "d" : $('#'+myBoardId).addClass('destroyer-demage');
                    break;
                case "e" : $('#'+myBoardId).addClass('patrol-boat-demage');
                    break;
            }
        }
    }
}

var opponentBoardArr = [];
for (var i = 0; i < 10; i++) {
    opponentBoardArr[i] = [];
    for (var j = 0; j < 10; j++) {
        opponentBoardArr[i][j] = result2.OpponentBoardBoard[i * 10 + j];
    }
}


// var baseUrl = "http://localhost:32033/api/";
var baseUrl = "http://battleshots-1.apphb.com/api/";
var gameAcces = false;
var selectBar = false;
var userName = localStorage.getItem('userName');
var animationSpeed = 1200;
var data = dataRepositories.get(baseUrl);
if (userName == null) {
    userName = "Example User";
}
$(document).ready(function () {
    checkGameAcces();
    $(function () {
        $('.draggable').draggable({
            containment: 'parent',
            cursor: 'move',
            revert: true
        });
    });

    $(function () {
        $('.grid-box').droppable({
            drop: handleDrop
        });
    });

    function handleDrop(event, ui) {
        ui.draggable.position({
            of: $(this),
            my: 'left top',
            at: 'left top'
        });
        ui.draggable.draggable('option', 'revert', false);
        ui.draggable.draggable("option", "grid", [52, 52]);
    }
});

function checkGameAcces(id) {
    if (localStorage.getItem("sessionKey") == null) {
        showLogin();
    } else {
        userName = localStorage.getItem('userName');
        $('#username').html(userName);
        $('#user-box').fadeIn(animationSpeed);
        if (selectBar == true) {
            showArena(id);
        } else {
            showBars();
        }
    }
}

function showLogin() {
    $('.basic-element').hide();
    $('#login-box').fadeIn(animationSpeed);
}
function showJoin() {
    $('.basic-element').hide();
    $('#join-box').fadeIn(animationSpeed);
}
function showArena(gameId) {
    $('.basic-element').hide();
    $('#arena').fadeIn(animationSpeed);
    $("<div />").load("Partials/arena-table-user.html", function (html) {
        $("main").append(html);
    });
    $("<div />").load("Partials/arena-table-enemy.html", function (html) {
        $("main").append(html);
    });

    data.battle.random(gameId).then(function (result) {
        data.battle.state(gameId).then(function (result2) {
            var id = result2.Id;
            DrowArena('mine-',result2,"MyBoard");
            DrowArena('opp-',result2,"OpponentBoard");
        }, function (err) {
            showError(err);
        });
    }, function (error) {
        showError(error);
    });
}

var game = $.connection.gameHub;
game.client.requestBoards = function (gameId) {
    alert("Request boards called! Game ID: " + gameId);
};

function showBars() {
    $('.basic-element').hide();
    $('#bars-box').fadeIn(animationSpeed);

    $('#bars-box').on("click", ".table-cell .btn", function (e) {
        var id = e.target.id;
        var pass = $(e.target).parent().parent().find("input").val();
        data.games.join(id, pass)
            .then(function (res) {
                selectBar = true;
                $.connection.hub.start()
                    .done(function () {
                        game.server.storeConnectionId(false, id);
                        game.server.informPlayer(id, false);
                    });
                selectBar = true;
                showArena(id);
            },
            function (err) {
                showError(err);
            });
    });
    $("<div />").load("Partials/lobby-tables.html", function (html) {
        data.games.open()
            .then(function (ress) {
                $(ress).each(function (el) {
                    var result = parseTemplate(html, { title: ress[el].Title, i: ress[el].Id, pwd: "" });
                    $("#bars-box").append(result);
                });
            }, function (error) {
                showError(error);
            });
        setInterval(function () {
            data.games.open()
                .then(function (res) {
                    var values = [];
                    $("#bars-box input").each(function () {
                        values.push($(arguments[1]).val());
                    });

                    $("#bars-box").html("");
                    $(res).each(function (el) {
                        var result = parseTemplate(html, { title: res[el].Title, i: res[el].Id, pwd: values[el] });
                        $("#bars-box").append(result);
                    });
                }, function (error) {
                    showError(error);
                });
        }, 5000);
    });

    $("#create-table-form").load("Partials/create-table-form.html", function () {
        $("#create-table-form").on("click", "#create-table-btn", function (e) {
            var tableName = $("#table-name").val();
            var tablePass = $("#table-pass").val();
            var confirmPass = $("#confirm-pass").val();

            if (tablePass == confirmPass) {
                var pass = SHA1(tablePass);
                data.games.newGame(tableName, tablePass)
                    .then(function (r) {
                        $("#table-name").val("");
                        $("#table-pass").val("");
                        $("#confirm-pass").val("");
                        $.connection.hub.start()
                            .done(function () {
                                game.server.storeConnectionId(true, r.Id);
                                selectBar = true;
                                checkGameAcces(r.Id);
                            });
                    }, function (err) {
                        showError(err);
                    });
            }
            else {
                showError('The passwords do not match.');
            }
        })
    });
}

function registerUser() {
    var loginName = $('#username-join').val();
    var userPass = $('#password-join').val();
    var userPass2 = $('#password-2-join').val();
    if (userPass === userPass2) {
        var pass = SHA1(userPass);

        var userData = {
            "username": loginName,
            "password": pass
        };
        httpRequester.postJson(baseUrl + 'account/register', userData, {})
            .then(function (userData) {
                var DataUserName = userData['Username'];
                var dataSessionKey = userData['SessionKey'];
                localStorage.setItem('userName', DataUserName);
                localStorage.setItem('sessionKey', dataSessionKey);

                checkGameAcces();
            }, function (error) {
                showError(error);
            });
    } else {
        showError('Incorrect password!');
    }

}

function checkLogin() {

    var loginName = $('#username-login').val();
    var userPass = $('#password-login').val();
    var passSha1 = SHA1(userPass);
    var data = {
        'username': loginName,
        'password': passSha1
    };
    httpRequester.postJson(baseUrl + 'account/login', data, {})
        .then(function (data) {

            var DataUserName = data['Username'];
            var dataSessionKey = data['SessionKey'];
            localStorage.setItem('userName', DataUserName);
            localStorage.setItem('sessionKey', dataSessionKey);

            checkGameAcces();
        }, function (error) {
            console.log(error);
            showError(error);
        });
}


function showError(error) {
    var text = '';
    if (typeof (error) == "object") {
        var err = JSON.parse(error.responseText);
        text = err['Message'];
    } else {
        text = error;
    }
    $('#error-dialog').html(text).dialog({ modal: true });
}

function logOut() {
    var sk = localStorage.getItem('sessionKey');
    var headers = {
        'X-SessionKey': sk
    }
    httpRequester.putJson(baseUrl + 'account/logout', {}, headers)
        .then(function (data) {
            $('#user-box').fadeOut(animationSpeed);
            localStorage.removeItem('userName');
            localStorage.removeItem('sessionKey');
            location.reload();
            checkGameAcces();
        }, function (error) {
            showError(error);
        });
}
