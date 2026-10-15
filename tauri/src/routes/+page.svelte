<script lang="ts">
  import type { PhysicalPosition } from "@tauri-apps/api/dpi";
  import { type UnlistenFn } from "@tauri-apps/api/event";
  import { getCurrentWebview } from "@tauri-apps/api/webview";
  import { onDestroy, onMount } from "svelte";

  let dropArea: HTMLDivElement | null = $state(null);
  let isDragOver = $state(false);
  let droppedPaths: string[] = $state([]);
  const dndState: "idle" | "dragOver" | "dropped" = $derived(
    isDragOver ? "dragOver" : droppedPaths.length > 0 ? "dropped" : "idle"
  );

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
  let destroyed = false;
  // Listeners are registered asynchronously, so the component may already be
  // destroyed when one resolves. Unlisten it right away in that case.
  const keep = (unlisten: UnlistenFn) => {
    if (destroyed) unlisten();
    else unlistens.push(unlisten);
  };

  onMount(async () => {
    const webview = getCurrentWebview();
    const appWindow = webview.window;

    // Watch DPI scale factor changes
    scaleFactor = await appWindow.scaleFactor();
    keep(
      await appWindow.onScaleChanged((event) => {
        scaleFactor = event.payload.scaleFactor;
      })
    );

    keep(
      await webview.onDragDropEvent((event) => {
        const payload = event.payload;
        if (payload.type === "enter" || payload.type === "over") {
          isDragOver = hitTest(payload.position, dropArea);
          return;
        }
        if (payload.type === "drop") {
          if (hitTest(payload.position, dropArea) && payload.paths.length > 0) {
            droppedPaths = payload.paths;
          }
        }
        isDragOver = false;
      })
    );
  });

  onDestroy(() => {
    destroyed = true;
    for (const unlisten of unlistens) {
      unlisten();
    }
  });

  const clearDroppedPaths = () => {
    droppedPaths = [];
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
  {#if dndState === "dropped"}
    <div>Files successfully dropped. Thank you! 🤗</div>
  {/if}

  {#if droppedPaths.length > 0}
    <ul>
      {#each droppedPaths as path}
        <li>{path}</li>
      {/each}
    </ul>
    <button onclick={clearDroppedPaths}>Clear</button>
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
