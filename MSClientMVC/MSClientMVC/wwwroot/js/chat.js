"use strict";

var signalRLink = "https://localhost:44359/clusterApiHub";

$(document).ready(function () {
    document.getElementById("link").disabled = true;
    document.getElementById("link").value = signalRLink;
});

var connection = new signalR.HubConnectionBuilder().withUrl(signalRLink).build();
connection.start().catch(function (err) {
    document.getElementById("result").innerText = err.toString();
    document.getElementById("result").style.background = "#ffe8e9";
    return console.error(err.toString());
});

connection.on("function.create", function (json) {
    if (document.getElementById("requestType").value == "fafSignalR" || document.getElementById("requestType").value == "fafHttp") {
        document.getElementById("result").innerText = JSON.stringify(json).slice(1, -1);
        document.getElementById("result").style.background = "#e2ffe2";
    }
});

connection.on("function.delete", function (json) {
    if (document.getElementById("requestType").value == "fafSignalR" || document.getElementById("requestType").value == "fafHttp") {
        document.getElementById("result").innerText = JSON.stringify(json).slice(1, -1);
        document.getElementById("result").style.background = "#e2ffe2";
    }
});

connection.on("function.update", function (json) {
    if (document.getElementById("requestType").value == "fafSignalR" || document.getElementById("requestType").value == "fafHttp") {
        document.getElementById("result").innerText = JSON.stringify(json).slice(1, -1);
        document.getElementById("result").style.background = "#e2ffe2";
    }
});

connection.on("function.run", function (json) {
    if (document.getElementById("requestType").value == "fafSignalR" || document.getElementById("requestType").value == "fafHttp") {
        var result = JSON.stringify(json).slice(1, -1);
        document.getElementById("result").innerText = result.replace(/(?:\\[rn]|[\r\n]+)+/g, "\n");
        document.getElementById("result").style.background = "#e2ffe2";
    }
});

connection.on("function.read", function (json) {
    if (document.getElementById("requestType").value == "fafSignalR" || document.getElementById("requestType").value == "fafHttp") {
        document.getElementById("result").innerText = JSON.stringify(json).slice(1, -1).replace(/(?:\\[rn]|[\r\n]+)+/g, "\n");
        document.getElementById("result").style.background = "#e2ffe2";
    }
});

connection.on("function.readall", function (json) {
    if (document.getElementById("requestType").value == "fafSignalR" || document.getElementById("requestType").value == "fafHttp") {
        var functions = JSON.stringify(json).slice(1, -1);
        document.getElementById("result").innerText = functions.replace(/(?:\\[rn]|[\r\n]+)+/g, "\n");
        document.getElementById("result").style.background = "#e2ffe2";
    }
});

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
    document.getElementById("result").style.background = "#ffffff";

    var functionDefinitions;
    switch (document.getElementById("operation").value) {
        case "create":
            if (isNotEmpty(document.getElementById("function").value) && isNotEmpty(document.getElementById("id").value) && isNotEmpty(document.getElementById("functionName").value)) {
                functionDefinitions = {
                    id: document.getElementById("id").value,
                    name: document.getElementById("functionName").value,
                    functionData: document.getElementById("function").value
                }
            }
            else {
                document.getElementById("result").innerText = "Missing input data!";
                document.getElementById("result").style.background = "#ffe8e9";
                return;
            }
            break;
        case "run":
            if (isNotEmpty(document.getElementById("functionName").value)) {
                functionDefinitions = {
                    id: document.getElementById("id").value,
                    name: document.getElementById("functionName").value,
                    functionData: document.getElementById("parameters").value
                }

            }
            else {
                document.getElementById("result").innerText = "Missing input data!";
                document.getElementById("result").style.background = "#ffe8e9";
                return;
            }
            break;
        case "delete":
            if (isNotEmpty(document.getElementById("functionName").value)) {
                functionDefinitions = {
                    id: document.getElementById("id").value,
                    name: document.getElementById("functionName").value,
                    functionData: document.getElementById("parameters").value
                }
            }
            else {
                document.getElementById("result").innerText = "Missing input data!";
                document.getElementById("result").style.background = "#ffe8e9";
                return;
            }
            break;
        case "update":
            if (isNotEmpty(document.getElementById("functionName").value) && isNotEmpty(document.getElementById("function").value)) {
                functionDefinitions = {
                    id: document.getElementById("id").value,
                    name: document.getElementById("functionName").value,
                    functionData: document.getElementById("function").value
                }
            }
            else {
                document.getElementById("result").innerText = "Missing input data!";
                document.getElementById("result").style.background = "#ffe8e9";
                return;
            }
            break;
        case "read":
            if (isNotEmpty(document.getElementById("functionName").value)) {
                functionDefinitions = {
                    id: document.getElementById("id").value,
                    name: document.getElementById("functionName").value,
                    functionData: document.getElementById("function").value
                }
            }
            else {
                document.getElementById("result").innerText = "Missing input data!";
                document.getElementById("result").style.background = "#ffe8e9";
                return;
            }
            break;
        case "readall":
            functionDefinitions = {
                id: document.getElementById("id").value,
                name: document.getElementById("functionName").value,
                functionData: document.getElementById("function").value
            }
            break;
        default:
            break;
    }
    var data = JSON.stringify(functionDefinitions);
    $('#spinner').show();

    if (document.getElementById("requestType").value == "fafSignalR" || document.getElementById("requestType").value == "rpcSignalR") {

        var hubMethod = document.getElementById("requestType").value == "fafSignalR" ? 'WriteMessage' : 'RPC';
        
        if (connection) {
            start();
            var promise = connection.invoke(
                hubMethod, //'WriteMessage',
                "client name",
                "functions." + document.getElementById("operation").value, //route,
                "unique message id defined by the caller", //messageid,
                data, //json,
                emptyGuid, //organisationId
                userContextToken,
                'All') //tracing
                .then((value) => {
                    if (document.getElementById("requestType").value == "rpcSignalR") {
                        HandleResponse(value);
                    }
                })
                .catch((err) => {
                    document.getElementById("result").innerText = err.toString();
                    document.getElementById("result").style.background = "#ffe8e9";
                    return console.error(err.toString());
                })
                .finally(() => {
                    end();
                    $('#spinner').hide();
                });
        }
        else {
            document.getElementById("result").innerText = "Missing connection. Press Ctrl+F5 to restart page.";
        }
    }
    else {
        var user1 = {
            scope: "clinetname",
            route: "functions." + document.getElementById("operation").value,
            messageid: "unique message id defined by the caller",
            json: data,
            organisationId: "00000000-0000-0000-0000-000000000000",
            userContextToken: "00000000-0000-0000-0000-000000000000",
            tracing: "All"
        };
        start();

        var jqxhr = $.ajax({
            type: "POST",
            data: JSON.stringify(user1),
            url: document.getElementById("link").value,
            contentType: "application/json",
            success: (value) => {
                if (document.getElementById("requestType").value == "rpcHttp") {
                    HandleResponse(value);
                }
            },
            error: (err) => {
                document.getElementById("result").innerText = err.toString();
                document.getElementById("result").style.background = "#ffe8e9";
            },
            complete: () => {
                end();
                $('#spinner').hide();
            }
        });
    }
    event.preventDefault();
});

function HandleResponse(response) {
    var returnMessageWrapper = JSON.parse(atob(response));
    var responceData = atob(returnMessageWrapper.Data).slice(1, -1);
    document.getElementById("result").innerText = responceData.replace(/(?:\\[rn]|[\r\n]+)+/g, "\n");
    document.getElementById("result").style.background = "#e2ffe2";
}

document.getElementById("operation").addEventListener("change", function (event) {
    document.getElementById("function").disabled = document.getElementById("operation").value == "create" || document.getElementById("operation").value == "update" ? false : true;
    document.getElementById("parameters").disabled = document.getElementById("operation").value == "run" ? false : true;
});

document.getElementById("requestType").addEventListener("change", function (event) {
    switch (document.getElementById("requestType").value) {
        case "fafHttp":
            document.getElementById("link").disabled = false;
            document.getElementById("link").value = "https://localhost:44359/api/FAF";
            break;
        case "rpcHttp":
            document.getElementById("link").disabled = false;
            document.getElementById("link").value = "https://localhost:44359/api/RPC";
            break;
        default:
            document.getElementById("link").disabled = true;
            document.getElementById("link").value = signalRLink;
            break;
    }
});
