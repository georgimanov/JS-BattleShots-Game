/// <reference path="http-requester.js" />
window.dataRepositories = (function () {
    var usernameDb = localStorage.getItem("userName");
    var sessionKeyDb = localStorage.getItem("sessionKey");

    var DataRepository = Class.create({
        init: function (baseUrl) {
            this.baseUrl = baseUrl;
            this.games = new GamesRepository(baseUrl + "games/");
        }
    });

    var GamesRepository = Class.create({
        init: function (url) {
            this.url = url;
        },

        newGame: function (title, password) {
            debugger;
            var game = {
                title: title,
                password: SHA1(password)
            };
            var headers = {
                "X-SessionKey": sessionKeyDb
            };

            return httpRequester.postJson(this.url + "new/", game, headers);
        },

        join: function (id, title, password) {
            var game = {
                title: title,
                password: SHA1(password)
            };
            var headers = {
                "X-SessionKey": sessionKeyDb
            };

            return httpRequester.postJson(this.url + "join/" + id, game, headers);
        },

        open: function () {
            return httpRequester.getJson(this.url + "open/", {});
        }
    });

    return {
        get: function (url) {
            return new DataRepository(url);
        }
    }
})();