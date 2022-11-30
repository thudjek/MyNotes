window.ShowModal = () => {
    var modal = new bootstrap.Modal(document.getElementById('modal'), {
        keyboard: false
    });

    modal.show();
}