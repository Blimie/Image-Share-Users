$(function () {
    $(".delete").on('click', function () {
        const imageId = $(this).data('image-id');
        $("#delete-image-id-hidden").val(imageId);
        $("#delete-modal").modal();
    });         
});