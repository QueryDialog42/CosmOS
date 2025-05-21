<?php
  session_start();
  include 'database.php';
  
  if ($_SERVER['REQUEST_METHOD'] == 'POST'){
      $username = $_POST['username'];
      $usermail = $_POST['usermail'];
      $password = password_hash($_POST['password'], PASSWORD_DEFAULT);

      try{
          // SQL query, username can not be duplicated
          $stmt = $conn->prepare("SELECT * FROM cosmosusers WHERE username = ?;");
          $stmt->bind_param('s', $username);
          $stmt->execute();
          $result = $stmt->get_result();

          // SQL query, usermail can not be duplicated
          $stmt2 = $conn->prepare("SELECT * FROM cosmosusers WHERE usermail = ?;");
          $stmt2->bind_param('s', $usermail);
          $stmt2->execute();
          $result2 = $stmt2->get_result();

          if ($result->num_rows > 0){
            echo "<script>alert('",$username," is already exist');</script>";
          }
          elseif ($result2->num_rows > 0){
            echo "<script>alert('",$usermail," is already exist');</script>";
          }
          else {
            $stmt2 = $conn->prepare('INSERT INTO cosmosusers VALUES (?, ?, ?);');
            $stmt2->bind_param('sss', $username, $usermail, $password);
            $stmt2->execute();
            header("Location: index.php");
            exit();
          }
        } catch(Exception $ex){
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
