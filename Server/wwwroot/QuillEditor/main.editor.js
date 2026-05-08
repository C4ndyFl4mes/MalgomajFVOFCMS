import Quill from "quill";
import BlotFormatter from "@enzedonline/quill-blot-formatter2";

Quill.register("modules/blotFormatter2", BlotFormatter);

let quillInstances = new Map();

export function initQuill(elementId, dotNetRef) {
	const quill = new Quill(`#${elementId}`, {
		modules: {
			toolbar: {
				container: [
                    [{ font: [] }, { size: [] }],
                    [{ header: [1, 2, 3, 4, 5, 6 ]}],
					['bold', 'italic', 'underline', 'strike'],
                    [{ color: [] }, { background: [] }],
                    [{ list: 'ordered' }, { list: 'bullet' }, { list: 'check' }],
					['blockquote', 'code-block'],
                    ['link', 'image'],
                    [{ align: [] }, { direction: 'rtl' }]
				],
                
				handlers: {
					image: () => dotNetRef.invokeMethodAsync("OpenImageSelector")
				}
			},
            blotFormatter2: {
                align: {
                    allowAligning: true,
                    alignments: ['left', 'center', 'right']
                },
                resize: {
                    allowResizing: true,
                    allowResizeModeChange: false,
                    useRelativeSize: true,
                    imageOversizeProtection: true,
                    minimumWidthPx: 120
                },
                image: {
                    registerImageTitleBlot: true,
                    autoHeight: true,
                    allowAltTitleEdit: false,
                    linkOptions: {
                        allowLinkEdit: false
                    }
                }
            }
		},
		placeholder: "Börja skapa innehåll här...",
		theme: "snow"
	});

	quillInstances.set(elementId, quill);

	quill.on("text-change", () => {
		const htmlContent = quill.root.innerHTML;
		dotNetRef.invokeMethodAsync("UpdateContent", htmlContent);
	});
}

export function getHTMLContent(elementId) {
	const quill = quillInstances.get(elementId);
	return quill ? quill.root.innerHTML : "Quill instance not initialized.";
}

export function setHTMLContent(elementId, htmlContent) {
	const quill = quillInstances.get(elementId);
	if (quill) {
		const delta = quill.clipboard.convert({ html: htmlContent });
        quill.setContents(delta, 'silent');
	}
	return quill === undefined ? "Quill instance not initialized." : "Content set successfully.";
}

export function insertImage(elementId, imageId) {
    const quill = quillInstances.get(elementId);
    if (quill) {
        const range = quill.getSelection();
        const imageUrl = `/images/${imageId}/jpg/desktop.jpg`
        quill.insertEmbed(range ? range.index : 0, 'image', imageUrl);
    }
}