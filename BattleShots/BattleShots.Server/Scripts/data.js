/// <reference path="../libs/_references.js" />
window.dataRepositories = (function () {
    var usernameDb = localStorage.getItem("username");
    var accessTokenDb = localStorage.getItem("accessToken");

    var DataRepository = Class.create({
        init: function (url) {
            this.url = url;
            this.users = new UsersRepository(url + "users/");
            this.appointments = new AppointmentsRepository(url + "appointments/");
            this.lists = new ListsRepository(url + "lists/");
            this.todos = new TodosRepository(url + "todos/");
        }
    });

    var UsersRepository = Class.create({
        init: function (url) {
            this.url = url;
        },

        register: function (username, email, password) {
            var user = {
                username: username,
                email: email,
                authCode: CryptoJS.SHA1(password).toString()
            };

            return httpRequester.postJson(this.url + "register", user)
            .then(function (data) {
                return httpRequester.postJson("api/auth/token", user);
            }, function (error) {
                $("#errors").html(JSON.parse(error.Message));
            })
            .then(function (data) {
                localStorage.setItem("username", data.username);
                usernameDb = data.username;
                localStorage.setItem("accessToken", data.accessToken);
                accessTokenDb = data.accessToken;
                return data.accessToken;
            }, function (error) {
                $("#errors").html(JSON.parse(error.Message));
            });
        },
        login: function (username, email, password) {
            var user = {
                username: username,
                email: email,
                authCode: CryptoJS.SHA1(password).toString()
            };

            return httpRequester.postJson("api/auth/token", user)
            .then(function (data) {
                localStorage.setItem("username", data.username);
                usernameDb = data.username;
                localStorage.setItem("accessToken", data.accessToken);
                accessTokenDb = data.accessToken;
                return data.accessToken;
            }, function (error) {
                $("#errors").html(JSON.parse(error.Message));
            });
        },
        logout: function () {
            if (!accessTokenDb) {
                return $("#errors").html(JSON.parse(error.Message));
            }

            var headers = {
                "X-accessToken": accessTokenDb
            };

            usernameDb = "";
            accessTokenDb = "";
            localStorage.clear();
            return httpRequester.putJson(this.url + "logout", "", headers);
        },

        currentUser: function () {
            return usernameDb;
        }
    });

    var ListsRepository = Class.create({
        init: function (url) {
            this.url = url;
        },

        create: function (title, todos) {
            var todoList = {
                title: title,
                todos: todos
            };
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.postJson(this.url, todoList, headers);
        },
        all: function () {
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.getJson(this.url, headers);
        },
        todos: function (listId) {
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.getJson(this.url + listId + "/todos", headers);
        },
        addTodo: function (listId, text, isDone) {
            var todo = {
                text: text,
                isDone: isDone
            };
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.postJson(this.url + listId + "/todos", todo, headers);
        },
        changeState: function (todoId) {
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.putJson("api/todos/" + todoId, "", headers);
        }
    });

    var AppointmentsRepository = Class.create({
        init: function (url) {
            this.url = url;
        },
        create: function (subject, description, appointmentDate, duration) {
            var appointment = {
                subject: subject,
                description: description,
                appointmentDate: appointmentDate,
                duration: duration
            };
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.postJson(this.url, appointment, headers);
        },
        all: function () {
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.getJson(this.url + "all/", headers);
        },
        coming: function () {
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.getJson(this.url + "comming/", headers);
        },
        today: function () {
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.getJson(this.url + "today/", headers);
        },
        current: function () {
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.getJson(this.url + "current/", headers);
        },
        byDate: function (date) {
            var headers = {
                "X-accessToken": accessTokenDb
            };

            return httpRequester.getJson(this.url + "?date=" + date, headers);
        },
    });

    var TodosRepository = Class.create({
        init: function (url) {
            this.url = url;
        }
    });

    return {
        get: function (url) {
            return new DataRepository(url)
        }
    }
})();