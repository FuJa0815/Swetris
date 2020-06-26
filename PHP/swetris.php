<?php
$pdo = new PDO('mysql:host=localhost;dbname=swetris', 'root', '');
if (isset($_POST['Score']) && isset($_POST['Id'])) {
	$sql = "INSERT INTO Leaderboard (DeviceId, Score) VALUES (?, ?);";
	$statement = $pdo->prepare($sql);
	$statement->execute(array($_POST['Id'], $_POST['Score']));
}
else {
	$sql = "SELECT Username AS Name, Score, Timestamp AS Time FROM leaderboard JOIN user ON(leaderboard.DeviceId = user.DeviceId) ORDER BY Score DESC LIMIT 10;";
	foreach ($pdo->query($sql) as $row) {
		echo $row['Name']."<br>".$row['Score']."<br>".$row['Time']."<br>";
	}
}