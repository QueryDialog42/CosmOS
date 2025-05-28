<?php

    $db_file = ''; // the path of users.db
    try {
        $conn = new PDO("sqlite:$db_file");
        
        // Hata ayıklama modu
        $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

        $stmt = $conn->prepare('CREATE TABLE IF NOT EXISTS cosmosusers(username TEXT UNIQUE, usermail TEXT UNIQUE, userpass TEXT NOT NULL);');
        $stmt->execute();
    } catch (PDOException $e) {
        echo "Connection failed: " . $e->getMessage();
    }
?>
