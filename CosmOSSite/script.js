document.addEventListener("DOMContentLoaded", function () {
  const currentPage = window.location.pathname.split("/").pop();

  if (currentPage === "main.html") {
    const profileToggle = document.getElementById("profileToggle");
    const profilePanel = document.getElementById("profilePanel");
    const signOutBtn = document.getElementById("signout");
    const signOutButton = document.getElementById("signoutButton");
    const refreshBtn = document.getElementById("refresh");
    const uploadFile = document.getElementById("uploadFile");
    const uploadFileBtn = document.getElementById("uploadBtn");
    const createFolderBtn = document.getElementById('createFolder');

    let chooseFolderFirstWarn = () => alert("Please choose a base folder first.");

    uploadFileBtn?.addEventListener('click', chooseFolderFirstWarn);
    uploadFile?.addEventListener('click', chooseFolderFirstWarn);
    createFolderBtn?.addEventListener('click', chooseFolderFirstWarn);

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
      window.location.href = "index.php";
    });

    signOutButton?.addEventListener("click", function (e) {
      e.preventDefault();
      localStorage.removeItem("user");
      window.location.href = "index.php";
    });

  // Choose Folder bölümü
  let folderHandle;
  let handleUploadFiles;
  let handleCreateFolder;
  let handleRefresh;
  const chooseFolderBtn = document.getElementById("chooseFolderBtn");
  chooseFolderBtn?.addEventListener("click", async () => {
    if (handleUploadFiles) {
      uploadFileBtn?.removeEventListener('click', handleUploadFiles);
      uploadFile?.removeEventListener('click', handleUploadFiles);
      createFolderBtn?.removeEventListener('click', handleCreateFolder);
      refreshBtn?.removeEventListener('click', handleRefresh);
    }

    folderHandle = await window.showDirectoryPicker();
    if (folderHandle) {

      uploadFileBtn?.removeEventListener('click', chooseFolderFirstWarn);
      uploadFile?.removeEventListener('click', chooseFolderFirstWarn);
      createFolderBtn?.removeEventListener('click', chooseFolderFirstWarn);

      doFileLogics(folderHandle);

      handleUploadFiles = () => uploadFiles(folderHandle);
      handleCreateFolder = () => createFolder(folderHandle);
      handleRefresh = () => doFileLogics(folderHandle);


      uploadFileBtn?.addEventListener('click', handleUploadFiles);
      uploadFile?.addEventListener('click', handleUploadFiles);
      createFolderBtn?.addEventListener('click', handleCreateFolder);
      refreshBtn?.addEventListener('click', handleRefresh);
      
      setBaseTitle(folderHandle.name);
    }
  });
  
    function doFileLogics(folderHandle){
      getFilesFromFolder(folderHandle);
      getFoldersFromFolder(folderHandle);
    }

    function setBaseTitle(foldername){
      const title = document.getElementById("basetitle");
      title.textContent = foldername;
      title.style.fontSize = "50px";
      title.style.fontWeight = "bold";
    }
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
async function uploadFiles(targetFolderHandle){
    const input = document.createElement('input');
    input.type = 'file';
    input.multiple = true; // Allow multiple file selection
    input.accept = '.txt,.rtf,.wav,.mp3,.exe,.png,.jpg,.mp4'; // optional
    input.onchange = async (event) => {
        const files = event.target.files;
        if (files.length === 0) {
            return;
        }
        for (const file of files) {
            const newFileHandle = await targetFolderHandle.getFileHandle(file.name, { create: true });
            
            // Create a writable stream to write the file data to the new location
            const writableStream = await newFileHandle.createWritable();
            await writableStream.write(file);
            
            // Close the writable stream
            await writableStream.close();
            addToRecent(file.name)
        }
      }
      input.click();
}
});

// create folder bölümü
async function createFolder(dirHandle) {
      try {
        const newFolderName = prompt("Enter the new folder name:");

        if (newFolderName){
          const newFolderHandle = await dirHandle.getDirectoryHandle(newFolderName, { create: true }); // The 'create:true' option means create if doesn't exist
          alert('Folder created successfully: ' + newFolderHandle.name);
        }
      } catch (error) {
        console.error('Error: ' + error.message);
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
            folders.push(entry);
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
            await folderHandle.removeEntry(foldername, {recursive: true});
        }
    });

    fileDiv.appendChild(image);
    fileDiv.appendChild(fileBtn);
    return fileDiv;
}

// Recently Added için yardımcı fonksiyon
function addToRecent(filename) {
  const recentContainer = document.getElementById("recentContainer");
  
  // Mevcut dosyaları kontrol et (max 20 dosya)
  if (recentContainer.children.length >= 20) {
    recentContainer.removeChild(recentContainer.lastChild);
  }
  
  const extension = getFileExtension(filename);
  const fileType = getFileType(extension);
  
  const recentCard = document.createElement("div");
  recentCard.className = `recent-card ${fileType}`;
  
  recentCard.innerHTML = `
    <div class="recent-icon">${getFileIcon(extension)}</div>
    <div class="recent-name">${filename}</div>
    <div class="recent-size">${Math.floor(Math.random() * 500) + 100} KB</div>
    <div class="recent-date">${new Date().toLocaleDateString()}</div>
  `;
  
  recentContainer.insertBefore(recentCard, recentContainer.firstChild);
}

// Dosya türüne göre class belirleme
function getFileType(extension) {
  switch(extension) {
    case 'jpg':
    case 'png':
    case 'gif':
      return 'img';
    case 'mp4':
    case 'mov':
      return 'vid';
    case 'mp3':
    case 'wav':
      return 'mus';
    case 'zip':
    case 'rar':
      return 'zip';
    default:
      return 'doc';
  }
}

// Dosya ikonlarını belirleme
function getFileIcon(extension) {
  const icons = {
    'jpg': '🖼️',
    'png': '🖼️',
    'gif': '🖼️',
    'mp4': '🎬',
    'mov': '🎬',
    'mp3': '🎵',
    'wav': '🎵',
    'zip': '📦',
    'rar': '📦',
    'pdf': '📄',
    'doc': '📄',
    'docx': '📄',
    'xls': '📊',
    'xlsx': '📊',
    'txt': '📝',
    'exe': '⚙️'
  };
  return icons[extension] || '📁';
}

// Account Settings
document.addEventListener("DOMContentLoaded", function () {
  const form = document.getElementById("personalDetailsForm");
  const restoreBtn = document.getElementById("restoreBtn");

  if (!form) return;

  // Eğer localStorage'da kayıtlı veri varsa inputlara yaz
  const saved = localStorage.getItem("accountDetails");
  if (saved) {
    const data = JSON.parse(saved);
    document.getElementById("fullName").value = data.fullName || "";
    document.getElementById("uniqueId").value = data.uniqueId || "";
    document.getElementById("email").value = data.email || "";
    document.getElementById("phone").value = data.phone || "";
    document.getElementById("designation").value = data.designation || "";
    document.getElementById("location").value = data.location || "";
  }

  // Form kaydedildiğinde
  form.addEventListener("submit", function (e) {
    e.preventDefault();

    const data = {
      fullName: document.getElementById("fullName").value,
      uniqueId: document.getElementById("uniqueId").value,
      email: document.getElementById("email").value,
      phone: document.getElementById("phone").value,
      designation: document.getElementById("designation").value,
      location: document.getElementById("location").value,
    };

    localStorage.setItem("accountDetails", JSON.stringify(data));
    alert("Changes saved succesfully!");
  });

  // Restore Default butonu
  restoreBtn?.addEventListener("click", function () {
  });
});

document.addEventListener("DOMContentLoaded", function () {
  const fullNameInput = document.getElementById("fullName");
  const emailInput = document.getElementById("emailInput");
  // const form = document.getElementById("personalDetailsForm");
  const resetBtn = document.getElementById("resetBtn");

  // Mevcut kullanıcı bilgilerini doldur
  const user = JSON.parse(localStorage.getItem("user")) || {
    name: "",
    email: "",
  };
  fullNameInput.value = user.name || "";
  emailInput.value = user.email || "";



  // Restore Default (isteğe bağlı)
  resetBtn.addEventListener("click", function () {
    fullNameInput.value = user.name || "";
    emailInput.value = user.email || "";
  });
});

document.addEventListener("DOMContentLoaded", () => {
    const user = JSON.parse(localStorage.getItem("user"));
    if (user) {
      const emailSpan = document.getElementById("email");
      const nameSpan = document.getElementById("profileName");

      if (emailSpan) emailSpan.textContent = user.email;
      if (nameSpan) nameSpan.textContent = user.name;
    }
  });


  // Reset Password Bölümü //

document.addEventListener("DOMContentLoaded", () => {
    const sidebarItems = document.querySelectorAll(".sidebar ul li");
    const sectionPersonal = document.getElementById("sectionPersonal");
    const sectionPassword = document.getElementById("sectionPassword");

    sidebarItems.forEach(item => {
        item.addEventListener("click", () => {
            sidebarItems.forEach(i => i.classList.remove("active"));
            item.classList.add("active");

            const section = item.getAttribute("data-section");
            if (section === "personal") {
                sectionPersonal.style.display = "block";
                sectionPassword.style.display = "none";
            } else {
                sectionPersonal.style.display = "none";
                sectionPassword.style.display = "block";
            }
        });
    });
});

// file card processes
async function getFilesFromFolder(folderHandle) {
  let txt = 0, rtf = 0, wav = 0, mp3 = 0, exe = 0, png = 0, jpg = 0, mp4 = 0, unknown = 0;

  const fileContainer = document.getElementById("fileContainer");
  const totalsize = document.getElementById("totalsize");
  const totalsizeinfo = document.getElementById("storageinfo");
  fileContainer.innerHTML = ""; // Clear previous files

  const files = await getFilesInFolder(folderHandle); // Get file handles
  let size = 0;
  
  // Process each file
  for (const fileHandle of files) {
      const file = await fileHandle.getFile();
      size += file.size;
      const filename = file.name;
      const extension = getFileExtension(filename);

      switch (extension) {
        case "txt": txt++; break;
        case "rtf": rtf++; break;
        case "wav": wav++; break;
        case "mp3": mp3++; break;
        case "exe": exe++; break;
        case "png": png++; break;
        case "jpg": jpg++; break;
        case "mp4": mp4++; break;
        default: unknown++;
      }

      const fileCard = createFileCard(filename, file.size, extension, folderHandle);
      fileContainer.appendChild(fileCard);
  }

  setDashboardFileCounts(txt, rtf, wav, mp3, exe, png, jpg, mp4, unknown);
  size = formatFileSize(size);
  totalsize.textContent = size;
  totalsizeinfo.textContent = size + " / 1TB";
}

function setDashboardFileCounts(
  txt, rtf, wav, mp3, exe, png, jpg, mp4, unknown
){
    const textfiles = document.getElementById("textfiles");
    const musicfiles = document.getElementById("musicfiles");
    const photofiles = document.getElementById("photofiles");
    const videofiles = document.getElementById("videofiles");
    const exefiles = document.getElementById("exefiles");

    const smalltextfiles = document.getElementById("smalltextfiles");
    const smallmusicfiles = document.getElementById("smallmusicfiles");
    const smallphotofiles = document.getElementById("smallphotofiles");
    const smallvideofiles = document.getElementById("smallvideofiles");

    const totaltextfiles = document.getElementById("totaltextfiles");
    const totalRTFfiles = document.getElementById("totalRTFfiles");
    const totalwavfiles = document.getElementById("totalwavfiles");
    const totalmp3files = document.getElementById("totalmp3files");
    const totalexefiles = document.getElementById("totalexefiles");
    const totaljpgandpngfiles = document.getElementById("totaljpgandpngfiles");
    const totalvideofiles = document.getElementById("totalvideofiles");

    const smallothers = document.getElementById("smallothers");

    textfiles.textContent = txt + rtf;
    totaltextfiles.textContent = txt;
    totalRTFfiles.textContent = rtf;
    smalltextfiles.textContent = txt + rtf + " Files";

    musicfiles.textContent = wav + mp3;
    totalmp3files.textContent = mp3;
    totalwavfiles.textContent = wav;
    smallmusicfiles.textContent = wav + mp3 + " Files";

    photofiles.textContent = png + jpg;
    totaljpgandpngfiles.textContent = png + jpg;

    smallphotofiles.textContent = png + jpg + " Files";

    videofiles.textContent = mp4;
    totalvideofiles.textContent = mp4;
    smallvideofiles.textContent = mp4 + " Files";

    exefiles.textContent = exe;
    totalexefiles.textContent = exe;
    smallothers.textContent = exe + unknown + " Files";
}

// cards for MyFiles
function createFileCard(filename, fileSize, extension, folderHandle) {
  const fileCard = document.createElement("div");
  fileCard.className = "file-card";
  
  const iconContainer = document.createElement("div");
  iconContainer.className = "file-icon-container";
  
  const image = createFileIcon(extension);
  iconContainer.appendChild(image);
  
  const nameEl = document.createElement("div");
  nameEl.className = "file-name";
  nameEl.textContent = filename;
  
  const sizeEl = document.createElement("div");
  sizeEl.className = "file-size";
  sizeEl.textContent = formatFileSize(fileSize);
  
  const actionsEl = document.createElement("div");
  actionsEl.className = "file-actions";
  
  const deleteBtn = document.createElement("button");
  deleteBtn.className = "file-action-btn delete";
  deleteBtn.textContent = "Delete";
  deleteBtn.addEventListener("click", async (e) => {
    e.stopPropagation();
    if (confirm(`Do you want to delete ${filename}?`)) {
      await folderHandle.removeEntry(filename);
      fileCard.remove();
    }
  });
  
  
  const menuBtn = document.createElement("button");
  menuBtn.className = "context-menu-btn";
  menuBtn.innerHTML = "⋮";
  menuBtn.addEventListener("click", (e) => {
    e.stopPropagation();
    // Context menu açılacak
    alert("More actions will be added here");
  });
  
  actionsEl.appendChild(deleteBtn);
  fileCard.appendChild(menuBtn);
  fileCard.appendChild(iconContainer);
  fileCard.appendChild(nameEl);
  fileCard.appendChild(sizeEl);
  fileCard.appendChild(actionsEl);
  
  return fileCard;
}

function formatFileSize(bytes) {
  if (bytes < 1024) return bytes + " B";
  else if (bytes < 1048576) return (bytes / 1024).toFixed(1) + " KB";
  else if (bytes < 1073741824) return (bytes / 1048576).toFixed(1) + " MB";
  else return (bytes / 1073741824).toFixed(1) + " GB";
}


async function getFoldersFromFolder(selectedfolderHandle) {
  let foldercount = 0;
  const folderfiles = document.getElementById("folderfiles");
  const folderContainer = document.getElementById("folderContainer");
  folderContainer.innerHTML = ""; // Clear previous files

  const folders = await getFoldersInFolder(selectedfolderHandle); // Get folder handles

  // Process each folder
  for (const folderHandle of folders) {
      const foldername = folderHandle.name;
      const folderCard = createFolderCard(foldername, selectedfolderHandle);
      folderContainer.appendChild(folderCard);
      foldercount++;
  }

  folderfiles.textContent = foldercount;
}

// cards for Folders
function createFolderCard(foldername, folderHandle) {
  const folderCard = document.createElement("div");
  folderCard.className = "file-card";
  
  const iconContainer = document.createElement("div");
  iconContainer.className = "file-icon-container";
  
  const image = document.createElement("img");
  image.src = "SystemSources/folder2.png";
  image.className = "fileImg";
  iconContainer.appendChild(image);
  
  const nameEl = document.createElement("div");
  nameEl.className = "file-name";
  nameEl.textContent = foldername;
  
  const actionsEl = document.createElement("div");
  actionsEl.className = "file-actions";
  
  const deleteBtn = document.createElement("button");
  deleteBtn.className = "file-action-btn delete";
  deleteBtn.textContent = "Delete";
  deleteBtn.addEventListener("click", async (e) => {
    e.stopPropagation();
    if (confirm(`Do you want to delete ${foldername}?`)) {
      await folderHandle.removeEntry(foldername, {recursive: true});
      folderCard.remove();
    }
  });
  
  const menuBtn = document.createElement("button");
  menuBtn.className = "context-menu-btn";
  menuBtn.innerHTML = "⋮";
  menuBtn.addEventListener("click", (e) => {
    e.stopPropagation();
    // Context menu açılacak
    alert("More actions will be added here");
  });
  
  actionsEl.appendChild(deleteBtn);
  folderCard.appendChild(menuBtn);
  folderCard.appendChild(iconContainer);
  folderCard.appendChild(nameEl);
  folderCard.appendChild(actionsEl);
  
  return folderCard;
}

