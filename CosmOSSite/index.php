<?php
    session_start();
    include 'database.php';

    if(isset($_COOKIE['allowed'])){
        echo "<script>alert('Your password changed successfully. Now relog in with your new password.');</script>";
        setcookie('allowed', '', time() - 1000);
    }

    if ($_SERVER['REQUEST_METHOD'] == 'POST') {
        $inputmail = $_POST['usermail'];
        $inputpass = $_POST['password'];

        $stmt = $conn->prepare('SELECT * FROM cosmosusers WHERE usermail = ?');
        $stmt->bind_param('s', $inputmail);
        $stmt->execute();
        $result = $stmt->get_result();

        if ($result->num_rows == 0){
            echo '<script>alert("Wrong mail");</script>';
        }
        else{
            while($row = $result->fetch_assoc()){
                $hashedpass = $row['userpass'];
                $usermailname = $row['username'];
            }
            if (password_verify($inputpass, $hashedpass)){
                $_SESSION['username'] = $usermailname;
                $_SESSION['usermail'] = $inputmail;

                header('Location: main.html');
                exit();
            }
            else{
                echo "<script>alert('Wrong password')</script>";
            }
        }
    }
?>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <title>Login</title>
    <link rel="stylesheet" href="style.css" />
    <script src="script.js" defer></script>
</head>

<body>
    <div class="auth-container">
        <h2>Login</h2>
        <form id="loginForm" method="post">
            <input type="email" id="email" name="usermail" placeholder="Email" required />
            <input type="password" id="password" name="password" placeholder="Password" required />
            <button type="submit" id="loginBtn">Login</button>
        </form>
        <p>Don't have an account? <a href="register.php">Register</a></p>
    </div>
</body>
</html>