set dir=%~dp0
set apk=%~1

cd /d %dir%

java -jar apksigner.jar -appname ���� -keystore debug.keystore -alias androiddebugkey -pswd android "%apk%"

::����˵����
::-appname����ǩ����Ӧ�ó���������ѡ�������鲻ͬ��APP���϶�Ӧ��app��������Ϊ���ģ��������ڡ����١� 
::-keystore�����.keystoreǩ���ļ�
::-alias�����ǩ������
::-pswd�������Ӧǩ�������룬���������ǣ�android ��ѡ����������ǩ����ʱ����Ҫ�ֶ�����
::������ǩ����apk·������Ŀ¼·�������������Ŀ¼��������ǩ����

