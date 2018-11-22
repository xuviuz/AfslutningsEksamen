"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44359/clusterApiHub").build();
connection.start().catch(function (err) {
    document.getElementById("result").innerText = err.toString();
    return console.error(err.toString());
});


connection.on("function.create", function (json) {
    document.getElementById("result").innerText = JSON.stringify(json).slice(1, -1);
});

connection.on("function.delete", function (json) {
    document.getElementById("result").innerText = JSON.stringify(json).slice(1, -1);
});

connection.on("function.update", function (json) {
    document.getElementById("result").innerText = JSON.stringify(json).slice(1, -1);
});

connection.on("function.run", function (json) {
    document.getElementById("result").innerText = JSON.stringify(json).slice(1, -1);
});

connection.on("function.read", function (json) {
    document.getElementById("result").innerText = JSON.stringify(json).slice(1, -1).replace(/(?:\\[n]|[\n]+)+/g, "\n");
});

connection.on("function.readall", function (json) {
    var functions = JSON.stringify(json).slice(1, -1);
    document.getElementById("result").innerText = functions.replace(/(?:\\[rn]|[\r\n]+)+/g, "\n");
});


var settings = {
    Servicename: "",
    RefreshRateInSeconds: "60",
    FilterSeverity: "Information"
};

var user = {
    Userid: "00000000-0000-0000-0000-000000000000",//"85c5ced7-bc60-1805-e70d-d7ea8c0ff7c7",
    Username: "username",
    Email: "test@test.dk",
    Mobile: "+45 12345678",
    Metadata: [],
    Organisations: [{ id: "00000000-0000-0000-0000-000000000000" }, { id: "00000000-0000-0000-0000-0000000000a0" }, { id: "00000000-0000-0000-0000-0000000000b0" }, { id: "00000000-0000-0000-0000-0000000000c0" }]
};

var userContextToken = '00000000-0000-0000-0000-000000000000';
var emptyGuid = '00000000-0000-0000-0000-000000000000';

var startTime, endTime;

function start() {
    startTime = new Date();
};

function end() {
    endTime = new Date();
    var timeDiff = endTime - startTime; //in ms
    console.log(Math.round(timeDiff) + " ms");
}

function isNotEmpty(val) {
    return ((val !== '') && (val !== undefined) && (val.length > 0) && (val !== null));
}

document.getElementById("postButton").addEventListener("click", function (event) {
    document.getElementById("result").innerText = "";
    if (connection) {
        var functionDefinitions;
        switch (document.getElementById("operation").value) {
            case "create":
                if (isNotEmpty(document.getElementById("function").value) && isNotEmpty(document.getElementById("functionId").value) && isNotEmpty(document.getElementById("functionName").value)) {
                    functionDefinitions = {
                        id: document.getElementById("functionId").value,
                        name: document.getElementById("functionName").value,
                        functionData: document.getElementById("function").value
                    }
                }
                else {
                    document.getElementById("result").innerText = "Missing input data!";
                    return;
                }
                break;
            case "run":
                if (isNotEmpty(document.getElementById("functionName").value)) {
                    functionDefinitions = {
                        id: document.getElementById("functionId").value,
                        name: document.getElementById("functionName").value,
                        functionData: document.getElementById("parameters").value
                    }
                }
                else {
                    document.getElementById("result").innerText = "Missing input data!";
                    return;
                }
                break;
            case "delete":
                if (isNotEmpty(document.getElementById("functionName").value)) {
                    functionDefinitions = {
                        id: document.getElementById("functionId").value,
                        name: document.getElementById("functionName").value,
                        functionData: document.getElementById("parameters").value
                    }
                }
                else {
                    document.getElementById("result").innerText = "Missing input data!";
                    return;
                }
                break;
            case "update":
                if (isNotEmpty(document.getElementById("functionName").value && isNotEmpty(document.getElementById("function").value))) {
                    functionDefinitions = {
                        id: document.getElementById("functionId").value,
                        name: document.getElementById("functionName").value,
                        functionData: document.getElementById("function").value
                    }
                }
                else {
                    document.getElementById("result").innerText = "Missing input data!";
                    return;
                }
                break;
            case "read":
                if (isNotEmpty(document.getElementById("functionName").value)) {
                    functionDefinitions = {
                        id: document.getElementById("functionId").value,
                        name: document.getElementById("functionName").value,
                        functionData: document.getElementById("function").value
                    }
                }
                else {
                    document.getElementById("result").innerText = "Missing input data!";
                    return;
                }
                break;
            case "readall":
                functionDefinitions = {
                    id: document.getElementById("functionId").value,
                    name: document.getElementById("functionName").value,
                    functionData: document.getElementById("function").value
                }
                break;
            default:
                break;
        }
        
            var data = JSON.stringify(functionDefinitions);
            start();
            var promise = connection.invoke(
                'WriteMessage',
                "client name",
                "functions." + document.getElementById("operation").value, //route,
                "unique message id defined by the caller", //messageid,
                data, //json,
                emptyGuid, //organisationId
                userContextToken,
                'All') //tracing
                .catch(function (err) {
                    return console.error(err.toString());
                });
            promise.then(function (value) {
                end();
            });
    }
    event.preventDefault();
});

document.getElementById("operation").addEventListener("change", function (event) {
    document.getElementById("function").disabled = document.getElementById("operation").value == "create" || document.getElementById("operation").value == "update" ? false : true;
    document.getElementById("parameters").disabled = document.getElementById("operation").value == "run" ? false : true;
});
