function showImageModal(src) {
    // Set the source of the modal image to the source of the clicked image
    document.getElementById('modalImage').src = src;
    // Show the modal
    $('#imagePreviewModal').modal('show');
}

function showVideoModal(src, type) {
    var video = document.getElementById('modalVideo');
    video.src = src;
    video.type = type;
    $('#videoPreviewModal').modal('show');
}

$('#videoPreviewModal').on('hidden.bs.modal', function (e) {
    var video = document.getElementById('modalVideo');
    video.pause(); // Pause the video when the modal is closed
    video.src = ""; // Reset source to ensure it stops loading
});