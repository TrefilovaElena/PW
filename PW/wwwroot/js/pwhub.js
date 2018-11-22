let token;		// токен
let username;	// имя пользователя
<script src="js/pwhub.js"></script>

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/balance", { accessTokenFactory: () => token })
    .build();


hubConnection.on("Balance", function (data) {

    document.getElementById("currentBalance").text = data;
  
});

/*
    hubConnection.invoke("Balance", message);
}); */
document.getElementById("newTransactionBtn").addEventListener("click", function (e) {
    //сюда вставить вызов баланса из бд
    let message = document.getElementById("currentBalance").text+5;
    hubConnection.invoke("Send", message);
});

hubConnection.on('Receive', function (message, userName) {

    console.log(message);
    console.log(userName);
});
//hubConnection.start();