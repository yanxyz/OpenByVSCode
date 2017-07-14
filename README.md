# OpenByVSCode

C# Console App，使用 VSCode 打开文件或目录

- 如果是文件，则从文件所在目录开始向上查找 `.vscode`, `.git` 等目录，若找到则打开它所在目录
- 如果是目录，则打开它

[Download](https://pan.baidu.com/s/1miqIlO8)

## 配置

配置文件 `OpenByVSCode.ini`，跟程序放在一起。

`OpenByVSCode -e` 打开配置文件编辑。示例

```ini
[OpenByVSCode]
vscode=code
#vscode=code-insiders
folders=.vscode,.git

# code 命令行参数，code -h
locale=en-US
disable_extensions=
```

以上配置也可以用在命令行中，并且命令行优先。

## Total Commander

将此程序添加到垂直工具条中

- Commands: `%COMMANDER_PATH%\Tools\OpenByVSCode\OpenByVSCode.exe`
- Parameters: `%P%S`
- Icon: `C:\Program Files (x86)\Microsoft VS Code\Code.exe`

勾选 "Run minimized"。

在 source panel 选中文件，然后点击上面添加的按钮。

## License

MIT (c) Ivan Yan
