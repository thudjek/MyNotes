window.addDropdownCloseEventListener = (dropdownObjRef, id) => {
    window.addEventListener('click', function (event) {
        var dropdownToggleElement = document.getElementById(id);
        var isClickInsideElement = dropdownToggleElement.contains(event.target);
        if (!isClickInsideElement) {
            dropdownObjRef.invokeMethod('Hide');
        }
    });
};