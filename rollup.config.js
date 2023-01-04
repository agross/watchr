import resolve from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';
import terser from '@rollup/plugin-terser';
import typescript from '@rollup/plugin-typescript';

export default {
  // Stuff loaded from the CDN.
  external: [
    'jquery',
    'xterm',
    'xterm-addon-fit',
    'xterm-addon-web-links',
    'css-element-queries',
    'fontfaceobserver',
  ],
  input: 'source/Web/wwwroot/scripts/index.ts',
  output: [
    {
      file: 'source/Web/wwwroot/scripts/index.js',
      format: 'iife',
      sourcemap: true,
      globals: {
        xterm: 'window',
        'xterm-addon-fit': 'FitAddon',
        'xterm-addon-web-links': 'WebLinksAddon',
        'css-element-queries': 'window',
        fontfaceobserver: 'FontFaceObserver',
      },
    },
    {
      file: 'source/Web/wwwroot/scripts/index.min.js',
      format: 'iife',
      sourcemap: true,
      globals: {
        xterm: 'window',
        'xterm-addon-fit': 'FitAddon',
        'xterm-addon-web-links': 'WebLinksAddon',
        'css-element-queries': 'window',
        fontfaceobserver: 'FontFaceObserver',
      },
      plugins: [terser({ sourceMap: true })],
    },
  ],
  plugins: [
    typescript(),
    commonjs({ extensions: ['.js', '.ts'] }),
    resolve({ browser: true }),
  ],
};
