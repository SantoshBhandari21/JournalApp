/* ================================
   WYSIWYG EDITOR FUNCTIONS
   ================================ */

/* Toggle formatting */
window.editorToggle = (type) => {
    const editor = document.getElementById("editor");
    if (!editor) return;

    editor.focus();

    switch (type) {
        case "bold":
            document.execCommand("bold");
            break;

        case "italic":
            document.execCommand("italic");
            break;

        case "heading":
            document.execCommand("formatBlock", false, "h1");
            break;

        case "list":
            document.execCommand("insertUnorderedList");
            break;
    }
};

/* Get current toolbar state */
window.getEditorState = () => {
    return {
        bold: document.queryCommandState("bold"),
        italic: document.queryCommandState("italic"),
        heading: document.queryCommandValue("formatBlock") === "h1",
        list: document.queryCommandState("insertUnorderedList")
    };
};

/* Get editor HTML content */
window.getEditorContent = () => {
    const editor = document.getElementById("editor");
    if (!editor) return "";
    return editor.innerHTML;
};

/* ✅ SET content when editing existing journal */
window.setEditorContent = (html) => {
    const editor = document.getElementById("editor");
    if (!editor) return;

    editor.innerHTML = html || "";
};