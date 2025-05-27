<?php
session_start();
include 'database.php';

if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $username = $_POST['username'];
    $usermail = $_POST['usermail'];
    $password = hash('sha256', $_POST['password']);

    try {
        // SQL sorgusu, kullanıcı adı tekrar edilemez
        $stmt = $conn->prepare("SELECT * FROM cosmosusers WHERE username = :username");
        $stmt->bindParam(':username', $username);
        $stmt->execute();
        $result = $stmt->fetch(PDO::FETCH_ASSOC);

        // SQL sorgusu, e-posta adresi tekrar edilemez
        $stmt2 = $conn->prepare("SELECT * FROM cosmosusers WHERE usermail = :usermail");
        $stmt2->bindParam(':usermail', $usermail);
        $stmt2->execute();
        $result2 = $stmt2->fetch(PDO::FETCH_ASSOC);

        if ($result) {
            echo "<script>alert('{$username} is already exist');</script>";
        } elseif ($result2) {
            echo "<script>alert('{$usermail} is already exist');</script>";
        } else {
            $stmt3 = $conn->prepare('INSERT INTO cosmosusers (username, usermail, userpass) VALUES (:username, :usermail, :password)');
            $stmt3->bindParam(':username', $username);
            $stmt3->bindParam(':usermail', $usermail);
            $stmt3->bindParam(':password', $password);
            $stmt3->execute();

            header("Location: index.php");
            exit();
        }
    } catch (Exception $ex) {
        echo "Error: " . $ex->getMessage();
    }
}
?>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <title>Register</title>
  <link rel="stylesheet" href="style.css" />
  <script src="script.js" defer></script>
</head>
<body>
  <div class="auth-container">
    <h2>Register</h2>
    <form id="registerForm" method="post">
      <input type="text" id="name" name="username" placeholder="Name" required />
      <input type="email" id="email" name="usermail" placeholder="Email" required />
      <input type="password" id="password" name="password" placeholder="Password" required />
      <button type="submit">Register</button>
    </form>
    <p>Already have an account? <a href="index.php">Login</a></p>
  </div>
</body>
</html>
