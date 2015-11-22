///<reference path="typings/tsd.d.ts" />

/// <summary>
/// Format a string in .NET style
/// </summary>
function formatReplace(format: string, fn: (m: string, n: number) => any) {
  return (format || new String()).replace(/\{\{|\}\}|\{(\d+)\}/g, fn);
}

export function formatString(format: string, ...rest: any[]): string {
  var args = Array.prototype.slice.call(arguments, 1);

  return formatReplace(format,
    (m, n) => {
      switch (m) {
        case '{{':
          return '{';
        case '}}':
          return '}';
        default:
          return args[n];
      }
    });
}

/// <summary>
/// Format a string in .NET style, and return an array containing that result and and unconsumed format args.
/// </summary>
/// <example>
/// formatStringAndConsume('foo {0}', 'bar', new Error()) == ['foo bar', Error object]
/// </example>
export function formatStringAndConsume(format: string, ...rest: any[]): any[] {
  var args = Array.prototype.slice.call(arguments, 1);

  var consumed = 0;

  var replaced = formatReplace(format,
    (m, n) => {
      switch (m) {
        case '{{':
          return '{';
        case '}}':
          return '}';
        default:
          ++consumed;
          return args[n];
      }
    });

  var unconsumed = args.slice(1 + consumed);

  return [replaced, ...unconsumed];
}
