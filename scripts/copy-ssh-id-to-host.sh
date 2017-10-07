#!/bin/bash -

if [[ -z $1 ]]
  then
    echo -e "\033[31m No user@host supplied.\033[0m"
    exit 1
fi

SSH_PORT=22

if [[ -n $2 ]]
  then
    SSH_PORT=$2
fi

ssh-copy-id -p ${SSH_PORT} -i "/c/Users/shams/.ssh/id_rsa_clrdbg.pub" -o "UserKnownHostsFile=/dev/null" -o "StrictHostKeyChecking=no" $1
exit 0