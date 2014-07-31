/// <reference path="http-requester.js" />
window.dataRepositories = (function () {
    var usernameDb = localStorage.getItem("userName");
    var sessionKeyDb = localStorage.getItem("sessionKey");

    var DataRepository = Class.create({
        init: function (baseUrl) {
            this.baseUrl = baseUrl;
            this.games = new GamesRepository(baseUrl + "games/");
            this.battle = new BattleRepository(baseUrl + "battle/");
        }
    });

    var GamesRepository = Class.create({
        init: function (url) {
            this.url = url;
        },

        newGame: function (title, password) {
            var game = {
                title: title,
                password: SHA1(password)
            };
            var headers = {
                "X-SessionKey": localStorage.getItem("sessionKey")
            };

            return httpRequester.postJson(this.url + "new/", game, headers);
        },

        join: function (id, password) {
            debugger;
            var game = {};
            if (password) {
                game.password = SHA1(password);
            }
            var headers = {
                "X-SessionKey": localStorage.getItem("sessionKey")
            };
            return httpRequester.postJson(this.url + "join/" + id, game, headers);
        },

        open: function () {
            return httpRequester.getJson(this.url + "open/", {});
        }
    });

    var BattleRepository = Class.create({
        init: function (url) {
            this.url = url;
        },

        place: function (id, password, ships) {
            var game = {
                ships: ships
            };

            if (password) {
                game.password = password;
            }

            var headers = {
                "X-SessionKey": localStorage.getItem("sessionKey")
            };

            return httpRequester.postJson(this.url + "place/" + id, game, headers);
        },

        state: function (id) {
            debugger;
            var headers = {
                "X-SessionKey": localStorage.getItem("sessionKey")
            };
            return httpRequester.getJson(this.url + "state/" + id, {}, headers);
        },

        random: function (id) {
            debugger;
            return httpRequester.postJson(this.url + "random/" + id, {}, {});
        },

        attack: function (id, row, col) {
            var game = {
                row: row,
                col: col
            };
            var headers = {
                "X-SessionKey": localStorage.getItem("sessionKey")
            };
            return httpRequester.postJson(this.url + "attack/" + id, game, headers);
        }
    });

    return {
        get: function (url) {
            return new DataRepository(url);
        }
    }
})();