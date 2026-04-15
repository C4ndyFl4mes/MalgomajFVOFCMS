import Quill from "quill";
import { toolbarOptions } from "./toolbar.js";

let quillInstances = new Map();

export function initQuill(elementId, dotNetRef) {
  const quill = new Quill(`#${elementId}`, {
    modules: {
      toolbar: toolbarOptions
    },
    placeholder: "Write something...",
    theme: "snow"
  });

  quillInstances.set(elementId, quill);

  quill.on("text-change", () => {
    const htmlContent = quill.root.innerHTML;
    dotNetRef.invokeMethodAsync("OnEditorChanged", htmlContent);
  });
}

export function getHTMLContent(elementId) {
  const quill = quillInstances.get(elementId);
  return quill ? quill.root.innerHTML : "Quill instance not initialized.";
}

export function setHTMLContent(elementId, htmlContent) {
  const quill = quillInstances.get(elementId);
  if (quill) {
    quill.root.innerHTML = htmlContent;
  }
  return quill === undefined ? "Quill instance not initialized." : "Content set successfully.";
}