function showImageModal(src) {
    // Set the source of the modal image to the source of the clicked image
    document.getElementById('modalImage').src = src;
    // Show the modal
    $('#imagePreviewModal').modal('show');
}