# CleanDb

日付のカラムを指定して、指定した期間より古いものをDELTEするアプリです。DBのコネクションストリングは別の別のアプリケーションのconfigから取得可能です。

## 使い方

```
-a, --appsettings    Use connection string in AppSettings section.

--help               Display this help screen.

--version            Display version information.

value pos. 0         Required. Path to config file.

value pos. 1         Required. Connection name.

value pos. 2         Required. Table Name.

value pos. 3         Required. Date column name.

value pos. 4         Required. Time offset. #h: hours | #d: days | #m months
```

### 例

/path/to/configのConnectionStrings["connectionName"]に接続し`TableName`の`column_name`が６ヶ月より古いものを削除する。


```
CleanDb /path/to/config connectionName TableName column_name 6m
```


/path/to/configのAppSettings["connectionName"]に接続し`TableName`の`column_name`が10日より古いものを削除する。`-a`オプションを指定するとAppSettingsから取得します。

```
CleanDb /path/to/config connectionName TableName column_name 10d -a
```

## 開発時のヒント

コマンドラインのパースに[これ](https://github.com/gsscoder/commandline/)を使用しています。バージョン１系ではオプションのみで単一の引数を扱えなかった（ValueAttributeがなかった）のでalpha版ですが、２系を使用しています。ドキュメントが下記にあります。

https://github.com/gsscoder/commandline/wiki/Latest-Version

Nu-getでPreを入れています。正式リリースまでにAPIが変わるかもしれないので注意してください。

