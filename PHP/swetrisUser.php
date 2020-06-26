<?php
$pdo = new PDO('mysql:host=localhost;dbname=swetris', 'root', '');
if (isset($_POST['Id'])) {
	if (isset($_POST['OtherName']) && isset($_POST['Act'])) {
		$sql = "";
		if($_POST['Act'] == "in") {
			$sql = "INSERT INTO `user_nemesis`(`ChallangerId`, `ChallangedId`) VALUES (:id,(SELECT DeviceId FROM `user` WHERE user.Username = :name));";
			
		} else {
			if($_POST['Act'] == "del") {
				$sql = "DELETE s.* FROM user_nemesis s INNER JOIN user u ON (s.ChallangerId = u.DeviceId OR s.ChallangedId = u.DeviceId) WHERE u.Username = :name AND (s.ChallangerId = :id OR s.ChallangedId = :id);";
			}
		}
		$statement = $pdo->prepare($sql);
		$res = $statement->execute(["id"=>$_POST['Id'], "name"=>$_POST['OtherName']]);
		if($statement->rowCount() == 0) {
			echo "err";
		}
	}
	else if (isset($_POST['get'])) {
		$sql = "SELECT user.Username FROM user WHERE user.DeviceId = :id";
		$statement = $pdo->prepare($sql);
		if($statement->execute(["id"=>$_POST['Id']])) {
			echo $statement->fetch()['Username'];
		}
	} 
	else if (isset($_POST['put']) && isset($_POST['name'])) {
		$sql = "INSERT INTO `user`(`DeviceId`, `Username`) VALUES(:id,:name) ON DUPLICATE KEY UPDATE Username=:name;";
		$statement = $pdo->prepare($sql);
		$statement->execute(["id"=>$_POST['Id'], "name"=>$_POST['name']]);
	}
	else if (isset($_POST['nem']) && isset($_POST['name'])) {
		$sql = "SELECT Username FROM (SELECT max(leaderboard.Score) as Score, user.Username FROM (SELECT ChallangedId AS Id FROM `user_nemesis` WHERE `ChallangerId` = :id UNION SELECT ChallangerId AS Id FROM `user_nemesis` WHERE `ChallangedId` = :id) T JOIN user ON(T.Id = user.DeviceId) JOIN leaderboard ON(T.Id = leaderboard.DeviceId) GROUP BY Username) A WHERE A.Score >= (SELECT MAX(score) FROM leaderboard WHERE leaderboard.DeviceId=(SELECT user.DeviceId FROM user WHERE user.Username = :name));";
		$statement = $pdo->prepare($sql);
		if($statement->execute(["id"=>$_POST['Id'], "name"=>$_POST['name']])) {
			while ($row = $statement->fetch()) {
				echo $row['Username']."<br>";
			}
		}
	}
	else {
		$sql = "SELECT user.Username FROM (SELECT ChallangedId AS Id FROM `user_nemesis` WHERE `ChallangerId` = :id UNION SELECT ChallangerId AS Id FROM `user_nemesis` WHERE `ChallangedId` = :id) T JOIN user ON(T.Id = user.DeviceId);";
		$statement = $pdo->prepare($sql);
		if($statement->execute(["id"=>$_POST['Id']])) {
			while ($row = $statement->fetch()) {
				echo $row['Username']."<br>";
			}
		}
	}
}