<?php
session_start();
include 'database.php';

$message = '';

if ($_SERVER['REQUEST_METHOD'] == 'POST') {
    $oldpassword = $_POST['oldpassword'];
    $newpassword = $_POST['newpassword'];
    $confirmpassword = $_POST['confirmpassword'];

    try {
        $stmt = $conn->prepare('SELECT userpass FROM cosmosusers WHERE usermail = :usermail');
        $stmt->bindParam(':usermail', $_SESSION['usermail']);
        $stmt->execute();
        $result = $stmt->fetch(PDO::FETCH_ASSOC);

        if ($result) {
            $oldpassworddb = $result['userpass'];

            if (hash('sha256', $oldpassword) == $oldpassworddb) {
                $message = 'Current password is entered wrong';
            } elseif ($newpassword != $confirmpassword) {
                $message = 'New password and confirm password is not the same';
            } else {
                $hashedNewPassword = hash('sha256', $newpassword);
                $stmt = $conn->prepare('UPDATE cosmosusers SET userpass = :newpassword WHERE usermail = :usermail');
                $stmt->bindParam(':newpassword', $hashedNewPassword);
                $stmt->bindParam(':usermail', $_SESSION['usermail']);
                $stmt->execute();

                setcookie('allowed', true, time() + 1000);
                session_destroy();
                header('Location: index.php');
                exit();
            }
        } else {
            $message = 'User  not found';
        }
    } catch (Exception $ex) {
        $message = 'Error: ' . $ex->getMessage();
    }
}
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8" />
    <title>Account Settings</title>
    <link rel="stylesheet" href="style.css" />
    <script src="script.js" defer></script>
</head>

<body>
    <a href="main.html" class="back-button">← Ana Sayfaya Dön</a>

    <div class="account-settings">
        <div class="sidebar">
            <ul>
                <li class="active" data-section="personal">Personal Details</li>
                <li data-section="password">Password Reset</li>
            </ul>
        </div>

        <div class="content">
            <!-- PERSONAL DETAILS SECTION -->
            <div id="sectionPersonal">
                <form id="personalDetailsForm">
                    <div class="form-grid">
                        <label>Full Name *</label>
                        <input type="text" id="fullName" value="" disabled placeholder="<?php echo isset($_SESSION['username']) ? htmlspecialchars($_SESSION['username']) : ''; ?>" />
                    </div>
                    <div>
                        <label>Unique ID *</label>
                        <input type="text" id="uniqueId" value="XXXXXX" disabled />
                    </div>
                    <div>
                        <label>Email ID *</label>
                        <input type="email" id="email" value="" disabled placeholder="<?php echo isset($_SESSION['usermail']) ? htmlspecialchars($_SESSION['usermail']) : ''; ?>"/>
                    </div>
                    <div>
                        <label>Mobile Number *</label>
                        <input type="text" id="phone" value="0000000000" disabled />
                    </div>
                    <div>
                        <label>Designation</label>
                        <input type="text" id="designation" value="Developer" disabled />
                    </div>
                    <div>
                        <label>Location</label>
                        <select id="location" disabled>
                            <option>United States</option>
                            <option selected>Turkey</option>
                            <option>Germany</option>
                            <option>India</option>
                        </select>
                    </div>
                    </div>
                    <div class="form-footer">
                        <button type="button" class="btn-outline">Restore Default</button>
                        <button type="button" class="btn-primary">Save Changes</button>
                    </div>
                </form>
            </div>

            <!-- PASSWORD RESET SECTION -->
            <div id="sectionPassword" style="display: none;">
                <form id="resetPasswordForm" method="post">
                    <div class="form-grid">
                        <div>
                            <label>Current Password *</label>
                            <input type="password" name="oldpassword" id="currentPassword" required />
                        </div>
                        <div>
                            <label>New Password *</label>
                            <input type="password" name="newpassword" id="newPassword" required />
                        </div>
                        <div>
                            <label>Confirm New Password *</label>
                            <input type="password" name="confirmpassword" id="confirmPassword" required />
                        </div>
                    </div>
                    <div class="form-footer">
                        <span id="passwordMessage" style="color: red; margin-right: 10px;"><?php echo htmlspecialchars($message); ?></span>
                        <button type="submit" class="btn-primary">Save Password</button>
                    </div>
                </form>
            </div>

        </div>
    </div>
</body>
</html>
