export const ReplaceStreamingEmbeddings = (chunk: string): string =>
  chunk
    .replace(/^[[,]/g, '')           // Remove leading [ or ,
    .replace(/^"/, '')                // Remove leading "
    .replace(/\]$/, '')               // Remove trailing ]
    .replace(/"$/, '')                // Remove trailing "
    .replace(/\\r\\n$/, '\n')         // Replace trailing \r\n with \n
    .replace(/\\n/g, '\n');           // Replace all \n with newline
