for i in `find /home/database/ -name "*.sql" | sort --version-sort`; do mysql -udocker -pdocker rest_with_asp_net < $i; done;
