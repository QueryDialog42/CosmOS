<?php
    $db_server = "localhost";
    $db_user = "root";
    $db_pass = "";

    // Create connection
    $conn = new mysqli($db_server, $db_user, $db_pass);
    // Check connection
    if ($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    }

    // Check if the database exists
    $db_name = "users";
    $result = $conn->query("SELECT SCHEMA_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = '$db_name'");
    if ($result->num_rows == 0) {

        // Create the database if it does not exist
        if ($conn->query("CREATE DATABASE $db_name") === false) {
            echo "Error creating database: " . $conn->error;
        }
    }
    // Create connection with database
    $conn = new mysqli($db_server, $db_user, $db_pass, $db_name);

    // Create table if does not exist
    $stmt = $conn->prepare('CREATE TABLE IF NOT EXISTS cosmosusers(username VARCHAR(255) UNIQUE, usermail VARCHAR(255) UNIQUE, userpass VARCHAR(255) NOT NULL);');
    $stmt->execute();
?>