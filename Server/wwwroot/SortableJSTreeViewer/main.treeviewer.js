import Sortable from "https://cdn.jsdelivr.net/npm/sortablejs@1.15.6/+esm";

const treeViewerInstances = new Map();

let menuEditorInstance = null;

export function initMenuEditor(rootId, dotNetRef) {
    destroyMenuEditor();

    const notInMenuList = document.getElementById("not-in-menu-list");
    if (!notInMenuList) return;

    const inMenuTree = document.getElementById("in-menu-tree");
    if (!inMenuTree) return;

    const sortables = [];

    const notifyMenuChange = () => {
        if (!dotNetRef) return;
        const tree = readContainer(inMenuTree);
        dotNetRef.invokeMethodAsync("OnMenuChanged", JSON.stringify(tree, null, 2));
    };

    // Make a tree container (inMenuTree root or any .tree-children) sortable.
    // When a node is dropped into the tree, its own .tree-children also needs initializing.
    function makeTreeSortable(container) {
        if (Sortable.get(container)) return; // already initialized

        const s = Sortable.create(container, {
            group: { name: "cms-menu", pull: true, put: true },
            animation: 140,
            handle: ".drag-handle",
            draggable: ".tree-node",
            fallbackOnBody: true,
            swapThreshold: 0.65,
            emptyInsertThreshold: 30,
            onAdd(evt) {
                initNodeContainers(evt.item);
                notifyMenuChange();
            },
            onEnd: notifyMenuChange,
            onRemove: notifyMenuChange,
        });

        sortables.push(s);
    }

    // Recursively initialize all .tree-children inside a node that aren't yet sortable
    function initNodeContainers(node) {
        for (const c of node.querySelectorAll(".tree-children")) {
            makeTreeSortable(c);
        }
    }

    // Init flat "not in menu" list — items can be dragged out and back, no nesting
    const notInMenuSortable = Sortable.create(notInMenuList, {
        group: { name: "cms-menu", pull: true, put: true },
        animation: 140,
        handle: ".drag-handle",
        draggable: ".tree-node",
        onAdd: notifyMenuChange,
        onEnd: notifyMenuChange,
        onRemove: notifyMenuChange,
    });

    // Init the in-menu tree root and every existing .tree-children inside it
    makeTreeSortable(inMenuTree);
    for (const c of inMenuTree.querySelectorAll(".tree-children")) {
        makeTreeSortable(c);
    }

    menuEditorInstance = { notInMenuSortable, sortables, dotNetRef };

    notifyMenuChange();
}

export function destroyMenuEditor() {
    if (!menuEditorInstance) return;

    menuEditorInstance.notInMenuSortable.destroy();
    for (const s of menuEditorInstance.sortables) {
        s.destroy();
    }

    menuEditorInstance = null;
}





export function initHierarchy(rootId, dotNetRef) {
    const root = document.getElementById(rootId);
    if (!root) return;

    destroyHierarchy(rootId);

    const sortables = [];
    const containers = [root, ...root.querySelectorAll(".tree-children")];

    const notifyTreeChange = () => {
        if (!dotNetRef) return;
        const tree = readContainer(root);
        dotNetRef.invokeMethodAsync("OnTreeChanged", JSON.stringify(tree, null, 2));
    };

    const sortableOptions = {
        group: "cms-hierarchy",
        animation: 140,
        handle: ".drag-handle",
        draggable: ".tree-node",
        fallbackOnBody: true,
        swapThreshold: 0.65,
        emptyInsertThreshold: 30,
        onEnd: notifyTreeChange
    };

    for (const container of containers) {
        sortables.push(Sortable.create(container, sortableOptions));
    }

    treeViewerInstances.set(rootId, { sortables, dotNetRef });
    notifyTreeChange();
}

export function destroyHierarchy(rootId) {
    const instance = treeViewerInstances.get(rootId);
    if (!instance) return;

    for (const sortable of instance.sortables) {
        sortable.destroy();
    }

    treeViewerInstances.delete(rootId);
}

function readContainer(container) {
    const children = [...container.children].filter((el) => el.classList.contains("tree-node"));
    return children.map(readNode);
}

function readNode(node) {
    const id = node.dataset.id ?? "";
    const label = node.querySelector(":scope > .node-row > .node-label")?.textContent?.trim() ?? "";
    const childContainer = node.querySelector(":scope > .tree-children");

    return {
        id,
        label,
        children: childContainer ? readContainer(childContainer) : []
    };
}