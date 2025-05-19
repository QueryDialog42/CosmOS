document.addEventListener("DOMContentLoaded", function () {
  const currentPage = window.location.pathname.split("/").pop();

  if (currentPage === "index.html") {
    const profileToggle = document.getElementById("profileToggle");
    const profilePanel = document.getElementById("profilePanel");
    const signOutBtn = document.getElementById("signout");
    const signOutButton = document.getElementById("signoutButton");
    const myFilesBtn = document.getElementById("myFilesBtn");
    const folders = document.getElementById("myFoldersBtn");

    const user = localStorage.getItem("user");
    if (!user) {
      window.location.href = "login.html";
    } else {
      const parsed = JSON.parse(user);
      const nameEl = document.getElementById("profileName");
      const emailEl = document.querySelector(".email");
      if (nameEl) nameEl.textContent = parsed.name;
      if (emailEl) emailEl.textContent = parsed.email;
    }

    profileToggle?.addEventListener("click", function (e) {
      e.stopPropagation();
      if (profilePanel) {
        profilePanel.style.display =
          profilePanel.style.display === "block" ? "none" : "block";
      }
    });

    document.addEventListener("click", function () {
      if (profilePanel) {
        profilePanel.style.display = "none";
      }
    });

    signOutBtn?.addEventListener("click", function (e) {
      e.preventDefault();
      localStorage.removeItem("user");
      window.location.href = "login.html";
    });

    signOutButton?.addEventListener("click", function (e) {
      e.preventDefault();
      localStorage.removeItem("user");
      window.location.href = "login.html";
    });

    myFilesBtn?.addEventListener('click', getFilesFromFolder);
    folders?.addEventListener('click', getFoldersFromFolder);
  }

  if (currentPage === "register.html") {
    const registerForm = document.getElementById("registerForm");
    registerForm?.addEventListener("submit", function (e) {
      e.preventDefault();

      const name = document.getElementById("name").value.trim();
      const email = document.getElementById("email").value.trim();
      const password = document.getElementById("password").value;

      let users = JSON.parse(localStorage.getItem("users")) || [];

      const nameExists = users.some(user => user.name === name);
      const emailExists = users.some(user => user.email === email);

      if (nameExists) {
        alert("This username is already taken.");
        return;
      }

      if (emailExists) {
        alert("This email is already registered.");
        return;
      }

      users.push({ name, email, password });
      localStorage.setItem("users", JSON.stringify(users));
      window.location.href = "login.html";
    });
  }

  if (currentPage === "login.html") {
    const loginForm = document.getElementById("loginForm");
    loginForm?.addEventListener("submit", function (e) {
      e.preventDefault();
      const email = document.getElementById("email").value.trim();
      const password = document.getElementById("password").value;
      let users = JSON.parse(localStorage.getItem("users")) || [];
      const user = users.find(user => user.email === email && user.password === password);
      if (user) {
        localStorage.setItem("user", JSON.stringify(user));
        window.location.href = "index.html";
      } else {
        alert("Invalid credentials");
      }
    });
  }

  document.querySelectorAll('.menu-item').forEach(button => {
    button.addEventListener('click', () => {
      const targetId = button.getAttribute('data-section');

      // Aktif buton vurgusu
      document.querySelectorAll('.menu-item').forEach(btn => btn.classList.remove('active'));
      button.classList.add('active');

      // Sayfaları gizle
      document.querySelectorAll('.page-section, .dashboard').forEach(section => {
        section.style.display = 'none';
      });

      // İlgili sayfayı göster
      const targetSection = document.getElementById(targetId);
      if (targetSection) {
        targetSection.style.display = 'block';
      }
    });
  });

  // Upload Bölümü
async function uploadFiles(){
  // Create an invisible file input element
    const input = document.createElement('input');
    input.type = 'file';
    input.multiple = true; // Allow multiple file selection
    // Optional: set accept attribute if you want to restrict file types
    input.accept = '.jpg,.png,.txt';
    // Listen for file selection
    input.onchange = async (event) => {
        const files = event.target.files; // Get the FileList directly
        // Check if any files were selected
        if (files.length === 0) {
            return;
        }
        // Get a handle for the target folder
        const targetFolderHandle = await window.showDirectoryPicker();
        // Move files to the target folder
        for (const file of files) {
            
            // Create a new file handle in the target folder
            const newFileHandle = await targetFolderHandle.getFileHandle(file.name, { create: true });
            
            // Create a writable stream to write the file data to the new location
            const writableStream = await newFileHandle.createWritable();
            
            // Write the file data to the new file
            await writableStream.write(file);
            
            // Close the writable stream
            await writableStream.close();
            addToRecent(file.name)
        }
      }
      input.click();
}
  const uploadFile = document.getElementById("uploadFile");
  const uploadFileBtn = document.getElementById("uploadBtn");

  uploadFileBtn?.addEventListener("click", uploadFiles);
  uploadFile?.addEventListener('click', uploadFiles);


});

// create folder bölümü
async function createFolder() {
      try {
        // Let user pick a directory
        const dirHandle = await window.showDirectoryPicker();
        // Name of the new folder to create
        const newFolderName = prompt("Enter the new folder name:");
        // Create a new subdirectory handle inside the chosen directory
        // The 'create:true' option means create if doesn't exist
        const newFolderHandle = await dirHandle.getDirectoryHandle(newFolderName, { create: true });

        alert('Folder created successfully: ' + newFolderHandle.name);
      } catch (error) {
        console.error('Error: ' + error.message);
      }
    }
    document.getElementById('createFolder').addEventListener('click', createFolder);



  //MyFiles bölümü
  async function getFilesFromFolder() {
    const folderHandle = await window.showDirectoryPicker();
    const fileContainer = document.getElementById("fileContainer");
    fileContainer.innerHTML = ""; // Clear previous files

    const files = await getFilesInFolder(folderHandle); // Get file handles

    // Process each file
    for (const fileHandle of files) {
        const file = await fileHandle.getFile();
        const filename = file.name;
        const extension = getFileExtension(filename);
        const image = createFileIcon(extension);
        const fileDiv = createFileDiv(filename, file.size, image, folderHandle);

        fileContainer.appendChild(fileDiv);
    }
}

//MyFolders bölümü
  async function getFoldersFromFolder() {
    const selectedfolderHandle = await window.showDirectoryPicker();
    const folderContainer = document.getElementById("folderContainer");
    folderContainer.innerHTML = ""; // Clear previous files

    const folders = await getFoldersInFolder(selectedfolderHandle); // Get file handles

    // Process each file
    for (const folderHandle of folders) {
        const folder = await folderHandle;
        const foldername = folder.name;
        const extension = getFileExtension(foldername);
        const image = createFileIcon(extension);
        const folderDiv = createFolderDiv(foldername, image, selectedfolderHandle);

        folderContainer.appendChild(folderDiv);
    }
}


// Helper function to get all files in the folder
async function getFilesInFolder(folderHandle) {
    const files = [];
    for await (const entry of folderHandle.values()) {
        if (entry.kind === 'file') {
            files.push(entry); // Collect file handles
        }
    }
    return files;
}

// Helper function to get all folder in the folder
async function getFoldersInFolder(folderHandle) {
    const folders = [];
    for await (const entry of folderHandle.values()) {
        if (entry.kind === 'directory') {
            folders.push(entry); // Collect folder handles
        }
    }
    return folders;
}

// Helper function to get file extension
function getFileExtension(filename) {
    return filename.includes('.') 
        ? filename.substring(filename.lastIndexOf('.') + 1).toLowerCase() 
        : '';
}

// Helper function to create file icon based on extension
function createFileIcon(extension) {
    const iconMap = {
        'txt': "SystemSources/textfile.png",
        'rtf': "SystemSources/rtffile.png",
        'wav': "SystemSources/soundwav48x48.png",
        'mp3': "SystemSources/soundmp348x48.png",
        'exe': "SystemSources/exepenguin.png",
        'png': "SystemSources/Picture96x96.png",
        'jpg': "SystemSources/JPGpic.png",
        'mp4': "SystemSources/Videoicon.png"
    };

    const image = document.createElement("img");
    image.setAttribute("src", iconMap[extension] || "SystemSources/unknown.png");
    image.setAttribute("class", "fileImg");
    return image;
}

// Helper function to create file div with buttons
function createFileDiv(filename, fileSize, image, folderHandle) {
    const fileDiv = document.createElement("div");
    const fileBtn = document.createElement("button");
    const sizeBtn = document.createElement("button");

    fileBtn.setAttribute("class", "fileBtn");
    sizeBtn.setAttribute("class", "fileBtn");
    
    fileBtn.textContent = filename;
    sizeBtn.textContent = `${Math.floor(fileSize / 1024)} KB`; // Display size in KB

    fileBtn.addEventListener("click", async () => {
        if (confirm(`Do you want to delete ${filename}?`)) {
            await folderHandle.removeEntry(filename);
        }
    });

    fileDiv.appendChild(image);
    fileDiv.appendChild(fileBtn);
    fileDiv.appendChild(sizeBtn);
    return fileDiv;
}

function createFolderDiv(foldername, image, folderHandle) {
    const fileDiv = document.createElement("div");
    const fileBtn = document.createElement("button");

    image.setAttribute("src", "SystemSources/folder2.png");
    fileBtn.setAttribute("class", "fileBtn");
    
    fileBtn.textContent = foldername;

    fileBtn.addEventListener("click", async () => {
        if (confirm(`Do you want to delete ${foldername}?`)) {
            await folderHandle.removeEntry(foldername);
        }
    });

    fileDiv.appendChild(image);
    fileDiv.appendChild(fileBtn);
    return fileDiv;
}


function addToRecent(filename){
  const recentDiv = document.getElementById("recentContainer");
  const fileDiv = document.createElement("div");
  const fileBtn = document.createElement("button");
  const image = createFileIcon(getFileExtension(filename));

  fileBtn.textContent = filename;
  fileBtn.setAttribute("class", "fileBtn");
  image.setAttribute("class", "fileImg");

  fileDiv.appendChild(image);
  fileDiv.appendChild(fileBtn);
  recentDiv.appendChild(fileDiv);
}

