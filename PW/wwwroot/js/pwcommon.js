$(document).ready(function () {
    ShowForm("#loginForm");
});

var tokenKey = "accessToken";

function ShowForm(FormForShow) {
    $("#loginForm").css("display", "none");
    $("#registrationForm").css("display", "none");
    $("#mainForm").css("display", "none");
    $(FormForShow).css("display", "block");
    }

function ClearTransactionForm() {
    document.getElementById("amount").value = "";
    document.getElementById("users").value = "";
    document.getElementById("selectedUserId").value = "";
    CheckTransactionBtn();
}

// создание строки для таблицы
var row = function (transaction) {
    return "<tr data-rowid='" + transaction.recipientId + "'><td>" + transaction.payeeName + "</td><td>" + transaction.recipientName + "</td>" +
        "<td>" + transaction.amount + "</td><td>" + transaction.balance + "</td> <td>" + transaction.timeOfTransaction + "</td> " +
        "<td>" + (transaction.repeat ? "<a class='repeatLink' data-id='" + transaction.recipientId + "' data-user='" + transaction.recipientName + "' data-amount='" +
        transaction.amount + "'>Repeat</a> " : "<a class='returnLink' data-id='" + transaction.payeeId + "' data-user='" + transaction.payeeName +
            "' data-amount='" + transaction.amount + "'>Return money to payee</a> ");
}


function GetCurrentBalance() {
    
    $.ajax({
        url: '/api/balance',
        type: 'Get',
        beforeSend: function (xhr) {
            var token = sessionStorage.getItem(tokenKey);
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {  $('.currentBalance').text(data); }
    });
}

function GetTransactions() {

    $("table tbody").find("tr").remove();

    $.ajax({
        url: '/api/account' ,
        type: 'GET',
        contentType: "application/json",
        beforeSend: function (xhr) {
            var token = sessionStorage.getItem(tokenKey);
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            var rows = "";
            $.each(data, function (index, tr) {
                rows += row(tr);
            })
            $("table tbody").append(rows);

        }


    });
}



$(function () {

    $('#newTransactionBtn').click(function (e) {
        e.preventDefault();
        var recipientId = document.getElementById("selectedUserId").value;
        if (recipientId !== "0") {
            var amount = document.getElementById("amount").value;
            $.ajax({
                type: 'POST',
                url: '/api/account/' + recipientId + '/' + amount,
                contentType: 'application/json; charset=utf-8',
                beforeSend: function (xhr) {
                    var token = sessionStorage.getItem(tokenKey);
                    xhr.setRequestHeader("Authorization", "Bearer " + token);
                },
            }).success(function (data) {
                if (data.message !=='') alert(data.message)
                if (data.success) {
                    $("table tbody").prepend(row(data.transaction));
                    GetCurrentBalance();
                    ClearTransactionForm();
                }
                
            }).error(function (data) {
                $('.errors').text(data);
            });
        }
    });
});

$(function () {

    $('#submitRegistration').click(function (e) {
        e.preventDefault();
        $.ajax({
            type: 'POST',
            url: '/api/user',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                grant_type: 'password',
                userName: $('#nameRegistration').val(),
                email: $('#emailRegistration').val(),
                password: $('#passwordRegistration').val(),
                confirmPassword: $('#confirmPasswordRegistration').val()
            })
        }).success(function (data) {
            if (data.message !== '') alert(data.message);
            if (data.success) {
                $('#emailLogin').val($('#emailRegistration').val()),
                $('#passwordLogin').val($('#passwordRegistration').val()),
                $('.errors').text('');
                ShowForm("#loginForm");
            }
        }).fail(function (data) {
            $('.errors').text(data);
        });
    });
});


$('#submitLogin').click(function (e) {
    e.preventDefault();
    $.ajax({
        type: 'POST',
        url: '/authenticate',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify({
            grant_type: 'password',
            userName: $('#emailLogin').val(),
            email: $('#emailLogin').val(),
            password: $('#passwordLogin').val(),
            confirmPassword: $('#passwordLogin').val()
        })
    }).success(function (data) {
        sessionStorage.setItem(tokenKey, data.token);
        $('.userName').text('Welcome '+data.username);
        ClearTransactionForm();
        GetCurrentBalance();
        GetTransactions();
        ShowForm("#mainForm");
        $('.errors').text('');
    }).fail(function (ex) {
        $('.errors').text(ex);
        });


});

$("#logOut").click(function (e) {
    e.preventDefault();
    sessionStorage.removeItem(tokenKey);
    ClearTransactionForm();
    ShowForm("#loginForm");
    $('.errors').text('');
});

$("#registration").click(function (e) {
    e.preventDefault();
    ShowForm("#registrationForm");
    sessionStorage.removeItem(tokenKey);
});

$("body").on("change", "#amount", function () {
    CheckTransactionBtn();
})
$("body").on("input", "#amount", function () {
    CheckTransactionBtn();
})

$("body").on("click", ".returnLink", function () {
    var id = $(this).data("id");
    var amount = $(this).data("amount");
    var user = $(this).data("user");
    repeatTransaction(id, user, amount);
})

$("body").on("click", ".repeatLink", function () {
    var id = $(this).data("id");
    var amount = $(this).data("amount");
    var user = $(this).data("user");
    repeatTransaction(id, user, amount);
})

function repeatTransaction(id, user, amount) {
    document.getElementById("selectedUserId").value = id;   
    document.getElementById("amount").value = amount;
    document.getElementById("users").value = user;
    CheckTransactionBtn();
}

function CheckTransactionBtn() {
    if (( document.getElementById("selectedUserId").value === "") || (document.getElementById("amount").value ===""))
    {
        $('#newTransactionBtn').css("display", "none");
    }
    else
    {
        $('#newTransactionBtn').css("display", "block");
    }     
}

$(function () {
    $('#users').autocomplete(
        {
           source: function (request, response)
            {
                $.ajax({
                    type: 'Get',
                    contentType: "application/json; charset=utf-8",
                    beforeSend: function (xhr) {
                        var token = sessionStorage.getItem(tokenKey);
                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                    },
                    url: '/api/getusers',
                    data: { Term: request.term, Count: 20 },
                    success: function (data) {
                        response(data);
                        CheckTransactionBtn();
                    },
                    error: function () {
                        response([]);
                        CheckTransactionBtn();
                    }
                });
            },
            select: function (event, ui) {
                document.getElementById("selectedUserId").value = ui.item.id;
                CheckTransactionBtn();
            }

        }
    );
});



