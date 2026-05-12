import Sortable from "sortablejs";

const treeViewerInstances = new Map();

export function initMenuEditor(rootId, dotNetRef, options = {}) {
    const maxDepth = Number(options.maxDepth ?? 3);
    const key = `menu:${rootId}`;

    destroyMenuEditor(rootId);

    const root = document.getElementById(rootId);
    if (!root)
        return;

    const notInMenuList = root.querySelector("#not-in-menu-list");
    const inMenuTree = root.querySelector("#in-menu-tree");
    if (!notInMenuList || !inMenuTree)
        return;

    const treeSortables = [];

    let isNotifying = false;
    let pendingNotify = false;

    const notifyMenuChange = async () => {
        if (!dotNetRef)
            return;
    
        if (isNotifying) {
            pendingNotify = true;
            return;
        }

        isNotifying = true;

        try {
            do {
                pendingNotify = false;
                const inMenu = readContainer(inMenuTree);
                await dotNetRef.invokeMethodAsync("OnMenuChanged", JSON.stringify(inMenu));
            } while (pendingNotify)
        } finally {
            isNotifying = false;
        }
    }

    const computeNodeDepth = (node) => {
        let depth = 1;
        let currentNode = node;

        while (true) {
            const parentContainer = currentNode.parentElement;
            if (!parentContainer || parentContainer === inMenuTree)
                return depth;

            const parentNode = parentContainer.closest(".tree-node");
            if (!parentNode)
                return depth;

            depth += 1;
            currentNode = parentNode;
        }
    }

    const getSubtreeDepth = (node) => {
        const childContainer = node.querySelector(":scope > .tree-children");
        if (!childContainer)
            return 1;

        const childNodes = [...childContainer.children].filter(c => c.classList.contains("tree-node"));
        if (childNodes.length === 0)
            return 1;

        return 1 + Math.max(...childNodes.map(getSubtreeDepth));
    }

    const canDropWithinDepth = (toContainer, draggedNode) => {
        const containerDepth = toContainer === inMenuTree
            ? 1
            : (() => {
                const ownerNode = toContainer.closest(".tree-node");
                if (!ownerNode)
                    return 1;
                return computeNodeDepth(ownerNode) + 1;
            })();

        const subtreeDepth = getSubtreeDepth(draggedNode);
        const finalDepth = containerDepth + subtreeDepth - 1;

        return finalDepth <= maxDepth;
    }

    function makeTreeSortable(container) {
        if (Sortable.get(container))
            return;

        const s = Sortable.create(container, {
            group: {
                name: "cms-menu",
                pull: true,
                put: true
            },
            sort: true,
            animation: 140,
            handle: ".drag-handle",
            draggable: ".tree-node",
            fallbackOnBody: true,
            swapThreshold: 0.65,
            emptyInsertThreshold: 30,
            onMove(e) {
                return canDropWithinDepth(e.to, e.dragged);
            },
            onAdd(e) {
                const inserted = e.item.querySelector(":scope > .tree-children");
                if (inserted)
                    makeTreeSortable(inserted);
            },
            onEnd: notifyMenuChange
        });

        treeSortables.push(s);
    }

    const notInMenuSortable = Sortable.create(notInMenuList, {
        group: {
            name: "cms-menu",
            pull: true,
            put: true
        },
        sort: true,
        animation: 140,
        handle: ".drag-handle",
        draggable: ".tree-node",
        onMove(e) {
            if (e.from === notInMenuList && e.to === notInMenuList)
                return false; // Kan inte sorteras i samma lista.
            return true; // Får ta emot från in-menu-tree.
        },
        onEnd: notifyMenuChange
    });

    makeTreeSortable(inMenuTree);
    for (const c of inMenuTree.querySelectorAll(".tree-children"))
        makeTreeSortable(c);

    treeViewerInstances.set(key, {
        type: "menu",
        rootId,
        dotNetRef,
        notInMenuSortable,
        sortables: treeSortables
    });
}

export function destroyMenuEditor(rootId) {
    const key = `menu:${rootId}`;
    const instance = treeViewerInstances.get(key);

    if (!instance)
        return;

    instance.notInMenuSortable?.destroy();

    for (const s of instance.sortables ?? [])
        s.destroy();

    treeViewerInstances.delete(key);
}

function readContainer(container) {
    const children = [...container.children].filter(el => el.matches("li.tree-node"));
    return children.map(readNode);
}

function readNode(node) {
    const id = node.dataset.id ?? "";
    const menuItemId = node.dataset.menuItemId ?? "";
    const label = node.querySelector(":scope > .node-row > .node-label")?.textContent?.trim() ?? "";
    const childContainer = node.querySelector(":scope > .tree-children");

    return {
        id,
        menuItemId,
        label,
        children: childContainer ? readContainer(childContainer) : []
    };
}