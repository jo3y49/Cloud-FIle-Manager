var dropZone = document.getElementById('drop_zone');
var fileInput = document.getElementById('fileInput');
var filePreview = document.getElementById('filePreview');
var accumulatedFiles = []; // Initialize an array to hold accumulated files

function updateFileInput() {
    // Create a new DataTransfer object to accumulate files
    var dataTransfer = new DataTransfer();
    accumulatedFiles.forEach(file => dataTransfer.items.add(file));
    // Update the file input's files property
    fileInput.files = dataTransfer.files;
}

dropZone.addEventListener('click', function(e) {
    fileInput.click(); // Simulate click on the file input when drop zone is clicked
});

dropZone.addEventListener('dragover', function(e) {
    e.stopPropagation();
    e.preventDefault();
    e.dataTransfer.dropEffect = 'copy'; // Explicitly show this is a copy.
});

dropZone.addEventListener('drop', function(e) {
    e.stopPropagation();
    e.preventDefault();
    var files = e.dataTransfer.files; // Get the files that were dropped

    // Add the new files to the accumulatedFiles array
    for (var i = 0; i < files.length; i++) {
        if (!accumulatedFiles.find(f => f.name === files[i].name)) {
            accumulatedFiles.push(files[i]);
        }
    }

    updateFileInput(); // Update the file input to reflect the accumulated files
    handleFiles(accumulatedFiles); // Call a function to handle these files
});

fileInput.addEventListener('change', function(e) {
    var files = e.target.files; // Get the files from the input

    // Add the new files to the accumulatedFiles array
    for (var i = 0; i < files.length; i++) {
        if (!accumulatedFiles.find(f => f.name === files[i].name)) {
            accumulatedFiles.push(files[i]);
        }
    }

    updateFileInput(); // Ensure the file input is updated
    handleFiles(accumulatedFiles); // Call a function to handle these files
});

function handleFiles(files) {
    filePreview.innerHTML = ""; // Clear existing previews
    files.forEach(function(file) {
        // Create a container for each image
        var imgContainer = document.createElement("div");
        imgContainer.classList.add("img-container");

        var img = document.createElement("img");
        img.classList.add("obj");
        img.file = file;
        imgContainer.appendChild(img); // Add the image to the container

        filePreview.appendChild(imgContainer); // Add the container to the preview area

        var reader = new FileReader();
        reader.onload = (function(aImg) { return function(e) { aImg.src = e.target.result; }; })(img);
        reader.readAsDataURL(file);
    });
    
    var form = document.querySelector('form');
    if (accumulatedFiles.length > 0) {
        
        form.style.display = 'block'; // Ensure the form is also visible
        dropZone.classList.add("files-present");
    } else {
        
        form.style.display = 'none'; // Ensure the form is also visible
        dropZone.classList.remove("files-present");
    }
}