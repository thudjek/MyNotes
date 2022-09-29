window.initRichTextEditor = () => {

    var toolbarOptions = [
        ['bold', 'italic', 'underline', 'strike']
    ];

    var quillOptions = {
        modules: {
            toolbar: toolbarOptions
        },
        placeholder: 'Type here...',
        theme: 'snow'
    };

    var quill = new Quill("#editor", quillOptions);
}