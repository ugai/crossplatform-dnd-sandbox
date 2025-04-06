<script lang="ts">
  import type { PhysicalPosition } from "@tauri-apps/api/dpi";
  import { type UnlistenFn } from "@tauri-apps/api/event";
  import { getCurrentWebview } from "@tauri-apps/api/webview";
  import { onDestroy, onMount } from "svelte";

  let dropArea: HTMLDivElement | null = $state(null);
  let dndState: "idle" | "dragOver" | "dropped" = $state("idle");
  let droppedPaths: string[] = $state([]);

  let scaleFactor: number;
  const hitTest = (
    position: PhysicalPosition,
    dropArea: HTMLDivElement | null
  ): boolean => {
    if (!dropArea) return false;
    const lpos = position.toLogical(scaleFactor);
    const r = dropArea.getBoundingClientRect();
    const isInside =
      lpos.x >= r.left &&
      lpos.x <= r.right &&
      lpos.y >= r.top &&
      lpos.y <= r.bottom;
    return isInside;
  };

  const unlistens: UnlistenFn[] = [];
  onMount(async () => {
    const webview = getCurrentWebview();
    const window = webview.window;
    let _unlisten: UnlistenFn;

    // Watch DPI scale factor changes
    scaleFactor = await window.scaleFactor();
    _unlisten = await window.onScaleChanged((event) => {
      scaleFactor = event.payload.scaleFactor;
    });
    unlistens.push(_unlisten);

    _unlisten = await webview.onDragDropEvent((event) => {
      if (event.payload.type === "over") {
        dndState = hitTest(event.payload.position, dropArea)
          ? "dragOver"
          : "idle";
        return;
      } else if (event.payload.type === "drop") {
        dndState = hitTest(event.payload.position, dropArea)
          ? "dropped"
          : "idle";
        if (dndState === "dropped") {
          const paths = event.payload.paths;
          if (paths && paths.length > 0) {
            droppedPaths = paths;
          }
        }
        return;
      }
      dndState = "idle";
    });
    unlistens.push(_unlisten);
  });

  onDestroy(() => {
    for (const unlisten of unlistens) {
      unlisten();
    }
  });

  const clearDroppedPathss = () => {
    droppedPaths = [];
    dndState = "idle";
  };
</script>

<main class="container">
  <h1>Drag and Drop Test</h1>

  <div
    class={["drop-area", dndState === "dragOver" && "drag-over"]}
    bind:this={dropArea}
  >
    Drop here
  </div>

  <div>Status: {dndState}</div>
  {#if dndState == "dropped"}
    <div>Files successfully dropped. Thank you! 🤗</div>
  {/if}

  {#if droppedPaths.length > 0}
    <ul>
      {#each droppedPaths as path}
        <li>{path}</li>
      {/each}
    </ul>
    <button onclick={() => clearDroppedPathss()}>Clear</button>
  {/if}
</main>

<style>
  .drop-area {
    width: 300px;
    height: 200px;
    border: 2px dashed #ccc;
    display: flex;
    justify-content: center;
    align-items: center;
    margin-bottom: 1rem;
  }
  .drag-over {
    border-color: #007bff;
  }
</style>
